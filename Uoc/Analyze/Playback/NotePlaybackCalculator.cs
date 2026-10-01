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
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly PlaybackTimingCalculator timingCalculator;
        private readonly AnalysisSetting analysisSetting;
        private readonly MeasureIndex maxMeasureIndex;

        public NotePlaybackCalculator(SpeedMultiplierProvider speedMultiplierProvider, PlaybackTimingCalculator timingCalculator, AnalysisSetting analysisSetting, MeasureIndex maxMeasureIndex, IReadOnlyList<Layer> layers)
        {
            this.speedMultiplierProvider = speedMultiplierProvider ?? throw new ArgumentNullException(nameof(speedMultiplierProvider));
            this.timingCalculator = timingCalculator ?? throw new ArgumentNullException(nameof(timingCalculator));
            this.analysisSetting = analysisSetting ?? throw new ArgumentNullException(nameof(analysisSetting));
            this.maxMeasureIndex = maxMeasureIndex ?? throw new ArgumentNullException(nameof(maxMeasureIndex));
            if (layers == null) throw new ArgumentNullException(nameof(layers));
        }

        public long CalculateEnabledTiming(Position position)
        {
            return timingCalculator.CalculateTimingFromPosition(position);
        }

        public void ReleaseInstantiationCache()
        {
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
            var firstMeasureIndex = timingCalculator.CalculateMeasureIndexFromTiming(Math.Max(startTiming, 0));
            var lastMeasureIndex = timingCalculator.CalculateMeasureIndexFromTiming(Math.Max(endTiming, 0));
            var speedChangeEvents = speedMultiplierProvider.GetSpeedMultiplierChangeEventsAt(firstMeasureIndex, lastMeasureIndex, layer);

            var moveDistance = 0d;
            foreach (var speedChangeEvent in speedChangeEvents)
            {
                var speedChangeTiming = timingCalculator.CalculateTiming(speedChangeEvent.MeasureIndex.Value, speedChangeEvent.Tick.Value);
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
                return speedMultiplierProvider.GetMeasureStartSpeedMultiplier(0, layer);
            }

            var measureIndex = timingCalculator.CalculateMeasureIndexFromTiming(timing);
            var speedMultiplier = speedMultiplierProvider.GetMeasureStartSpeedMultiplier(measureIndex, layer);
            var speedChangeEvents = speedMultiplierProvider.GetSpeedMultiplierChangeEventsAt(measureIndex, measureIndex, layer);
            foreach (var speedChangeEvent in speedChangeEvents)
            {
                var speedChangeTiming = timingCalculator.CalculateTiming(speedChangeEvent.MeasureIndex.Value, speedChangeEvent.Tick.Value);
                if (speedChangeTiming > timing)
                {
                    break;
                }
                speedMultiplier = speedChangeEvent.SpeedMultiplier;
            }
            return speedMultiplier;
        }

        public long CalculateInstantiateTiming(Layer layer, long enabledTiming)
        {
            var minimumTiming = analysisSetting.MinimumTiming;
            var interval = analysisSetting.NotesInstantiationInterval;
            var maxTiming = (long)timingCalculator.CalculateMeasureStartTiming(maxMeasureIndex.Value + 1); // 最大のタイミングを、ノートが存在する最大小節の次の小節の開始点とする。
            for (var i = minimumTiming; i < maxTiming; i += interval)
            {
                var position = CalculateNotePosition(layer, enabledTiming, i);
                if (position <= 1f) // 位置が生成位置に到達した場合
                {
                    return Math.Max(i - interval, minimumTiming);
                }
            }
            return maxTiming;
        }
    }
}
