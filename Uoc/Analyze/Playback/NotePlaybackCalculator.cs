using System;
using System.Collections.Generic;
using Uoc.Analyze.Speed;
using Uoc.Chart;
using Uoc.Chart.Event;

namespace Uoc.Analyze.Playback
{
    // 譜面内の速度情報を共有して、ノート位置と生成時刻を計算する。
    internal class NotePlaybackCalculator
    {
        private const int InstantiationCacheCapacity = 4096;

        private readonly PlaybackTimingCalculator timingCalculator;
        private readonly AnalysisSetting analysisSetting;
        private readonly MeasureIndex maxMeasureIndex;
        private readonly Dictionary<int, IReadOnlyList<SpeedMultiplierChangeEvent>> speedChangeEventsByLayer = new();
        private readonly Dictionary<int, SpeedMultiplier> initialSpeedMultipliers = new();
        private readonly Dictionary<(int measureIndex, int tick), long> speedChangeTimings = new();
        private readonly object speedChangeTimingLock = new();
        private Dictionary<long, int>? instantiateMeasureIndices = new();
        private Queue<long>? instantiateMeasureOrder = new();
        private Dictionary<(int layer, long timing), SpeedMultiplier>? instantiateSpeedMultipliers = new();
        private Queue<(int layer, long timing)>? instantiateSpeedOrder = new();
        private long? maxTiming;

        public NotePlaybackCalculator(SpeedMultiplierProvider speedMultiplierProvider, PlaybackTimingCalculator timingCalculator, AnalysisSetting analysisSetting, MeasureIndex maxMeasureIndex, IReadOnlyList<Layer> layers)
        {
            if (speedMultiplierProvider == null) throw new ArgumentNullException(nameof(speedMultiplierProvider));
            this.timingCalculator = timingCalculator ?? throw new ArgumentNullException(nameof(timingCalculator));
            this.analysisSetting = analysisSetting ?? throw new ArgumentNullException(nameof(analysisSetting));
            this.maxMeasureIndex = maxMeasureIndex ?? throw new ArgumentNullException(nameof(maxMeasureIndex));
            if (layers == null) throw new ArgumentNullException(nameof(layers));
            foreach (var layer in layers)
            {
                initialSpeedMultipliers.Add(layer.Value, speedMultiplierProvider.GetMeasureStartSpeedMultiplier(0, layer));
                speedChangeEventsByLayer.Add(layer.Value, speedMultiplierProvider.GetSpeedMultiplierChangeEventsAt(0, maxMeasureIndex.Value, layer));
            }
        }

        public long CalculateEnabledTiming(Position position)
        {
            return timingCalculator.CalculateTimingFromPosition(position);
        }

        public void ReleaseInstantiationCache()
        {
            instantiateMeasureIndices = null;
            instantiateMeasureOrder = null;
            instantiateSpeedMultipliers = null;
            instantiateSpeedOrder = null;
        }

        public float CalculateNotePosition(Layer layer, long enabledTiming, long timing)
        {
            var basicSpeed = analysisSetting.BasicSpeed;

            /* 小節線以降のハイスピを無視する処理 */
            if (analysisSetting.IgnoreSpeedChangesAfterJudgeLine && timing > enabledTiming)
            {
                return (float)(((double)enabledTiming - timing) / basicSpeed.MoveDuration);
            }

            var startTiming = Math.Min(timing, enabledTiming);
            var endTiming = Math.Max(timing, enabledTiming);
            var moveDistance = CalculateMoveDistance(layer, startTiming, endTiming);
            var direction = timing > enabledTiming ? -1 : 1;
            return (float)(moveDistance / basicSpeed.MoveDuration * direction);
        }

        private double CalculateMoveDistance(Layer layer, long startTiming, long endTiming)
        {
            if (startTiming == endTiming)
            {
                return 0;
            }

            var currentTiming = startTiming;
            var currentSpeedMultiplier = GetSpeedMultiplierAt(layer, startTiming);
            var firstMeasureIndex = GetMeasureIndexFromTiming(Math.Max(startTiming, 0));
            var lastMeasureIndex = GetMeasureIndexFromTiming(Math.Max(endTiming, 0));
            if (firstMeasureIndex < 0) throw new ArgumentOutOfRangeException("startMeasureIndex");
            if (lastMeasureIndex < 0) throw new ArgumentOutOfRangeException("endMeasureIndex");
            if (firstMeasureIndex > lastMeasureIndex) throw new ArgumentException();
            var speedChangeEvents = speedChangeEventsByLayer[layer.Value];
            var firstEventIndex = FindMeasureEventStart(speedChangeEvents, firstMeasureIndex);
            var lastEventIndex = FindMeasureEventEnd(speedChangeEvents, lastMeasureIndex);

            var moveDistance = 0d;
            for (var i = firstEventIndex; i < lastEventIndex; i++)
            {
                var speedChangeEvent = speedChangeEvents[i];
                if (speedChangeEvent.MeasureIndex.Value == firstMeasureIndex && speedChangeEvent.Tick.Value == 0) continue;
                var speedChangeTiming = GetSpeedChangeTiming(speedChangeEvent);
                if (speedChangeTiming <= startTiming || speedChangeTiming >= endTiming)
                {
                    continue;
                }

                moveDistance += ((double)speedChangeTiming - currentTiming) * currentSpeedMultiplier.Multiplier;
                currentTiming = speedChangeTiming;
                currentSpeedMultiplier = speedChangeEvent.SpeedMultiplier;
            }

            moveDistance += ((double)endTiming - currentTiming) * currentSpeedMultiplier.Multiplier;
            return moveDistance;
        }

        private SpeedMultiplier GetSpeedMultiplierAt(Layer layer, long timing)
        {
            if (timing < 0)
            {
                return initialSpeedMultipliers[layer.Value];
            }

            var key = (layer.Value, timing);
            if (instantiateSpeedMultipliers != null && instantiateSpeedMultipliers.TryGetValue(key, out var cached)) return cached;
            var measureIndex = GetMeasureIndexFromTiming(timing);
            if (measureIndex < 0) throw new ArgumentOutOfRangeException(nameof(measureIndex));
            var speedChangeEvents = speedChangeEventsByLayer[layer.Value];
            var firstEventIndex = FindMeasureEventStart(speedChangeEvents, measureIndex);
            var lastEventIndex = FindMeasureEventEnd(speedChangeEvents, measureIndex);
            // 小節始点のイベントは最後の同位置イベントまで適用する。
            while (firstEventIndex < lastEventIndex && speedChangeEvents[firstEventIndex].Tick.Value == 0) firstEventIndex++;
            var speedMultiplier = firstEventIndex == 0 ? initialSpeedMultipliers[layer.Value] : speedChangeEvents[firstEventIndex - 1].SpeedMultiplier;
            for (var i = firstEventIndex; i < lastEventIndex; i++)
            {
                var speedChangeEvent = speedChangeEvents[i];
                var speedChangeTiming = GetSpeedChangeTiming(speedChangeEvent);
                if (speedChangeTiming > timing)
                {
                    break;
                }
                speedMultiplier = speedChangeEvent.SpeedMultiplier;
            }
            if (instantiateSpeedMultipliers != null)
            {
                if (instantiateSpeedMultipliers.Count == InstantiationCacheCapacity) instantiateSpeedMultipliers.Remove(instantiateSpeedOrder!.Dequeue());
                instantiateSpeedMultipliers.Add(key, speedMultiplier);
                instantiateSpeedOrder!.Enqueue(key);
            }
            return speedMultiplier;
        }

        private int GetMeasureIndexFromTiming(long timing)
        {
            if (instantiateMeasureIndices != null && instantiateMeasureIndices.TryGetValue(timing, out var cached)) return cached;
            var measureIndex = timingCalculator.CalculateMeasureIndexFromTiming(timing);
            if (instantiateMeasureIndices != null)
            {
                if (instantiateMeasureIndices.Count == InstantiationCacheCapacity) instantiateMeasureIndices.Remove(instantiateMeasureOrder!.Dequeue());
                instantiateMeasureIndices.Add(timing, measureIndex);
                instantiateMeasureOrder!.Enqueue(timing);
            }
            return measureIndex;
        }

        private long GetSpeedChangeTiming(SpeedMultiplierChangeEvent speedChangeEvent)
        {
            var key = (measureIndex: speedChangeEvent.MeasureIndex.Value, tick: speedChangeEvent.Tick.Value);
            lock (speedChangeTimingLock)
            {
                if (speedChangeTimings.TryGetValue(key, out var cached)) return cached;
                var timing = timingCalculator.CalculateTiming(key.measureIndex, key.tick);
                speedChangeTimings.Add(key, timing);
                return timing;
            }
        }

        private static int FindMeasureEventStart(IReadOnlyList<SpeedMultiplierChangeEvent> events, int measureIndex)
        {
            var lower = 0;
            var upper = events.Count;
            while (lower < upper)
            {
                var middle = lower + (upper - lower) / 2;
                if (events[middle].MeasureIndex.Value < measureIndex) lower = middle + 1;
                else upper = middle;
            }
            return lower;
        }

        private static int FindMeasureEventEnd(IReadOnlyList<SpeedMultiplierChangeEvent> events, int measureIndex)
        {
            var lower = 0;
            var upper = events.Count;
            while (lower < upper)
            {
                var middle = lower + (upper - lower) / 2;
                if (events[middle].MeasureIndex.Value <= measureIndex) lower = middle + 1;
                else upper = middle;
            }
            return lower;
        }

        public long CalculateInstantiateTiming(Layer layer, long enabledTiming)
        {
            var minimumTiming = analysisSetting.MinimumTiming;
            var interval = analysisSetting.NotesInstantiationInterval;
            maxTiming ??= (long)timingCalculator.CalculateMeasureStartTiming(maxMeasureIndex.Value + 1); // 最大小節の次の小節の開始点。
            var maximumTiming = maxTiming.Value;
            for (var i = minimumTiming; i < maximumTiming; i += interval)
            {
                var position = CalculateNotePosition(layer, enabledTiming, i);
                if (position <= 1f) // 位置が生成位置に到達した場合
                {
                    return Math.Max(i - interval, minimumTiming);
                }
            }
            return maximumTiming;
        }
    }
}
