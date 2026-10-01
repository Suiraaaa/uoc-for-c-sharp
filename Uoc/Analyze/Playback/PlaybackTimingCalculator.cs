using System;
using System.Collections.Generic;
using Uoc.Chart;
using Uoc.Chart.Event;

namespace Uoc.Analyze.Playback
{
    // 単体ノートとグループが同じTPB・丸め規則で時刻を計算する。
    internal class PlaybackTimingCalculator
    {
        private readonly BpmProvider bpmProvider;
        private readonly MeasureLengthProvider measureLengthProvider;
        private readonly Tpb tpb;
        private readonly object cacheLock = new();
        private readonly List<float> measureStartTimings = new() { 0f };
        private readonly List<long> measureBoundaryTimings = new();
        private bool canCacheTimings = true;

        public PlaybackTimingCalculator(BpmProvider bpmProvider, MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            this.bpmProvider = bpmProvider ?? throw new ArgumentNullException(nameof(bpmProvider));
            this.measureLengthProvider = measureLengthProvider ?? throw new ArgumentNullException(nameof(measureLengthProvider));
            this.tpb = tpb ?? throw new ArgumentNullException(nameof(tpb));
        }

        /// <summary>
        /// 指定された位置からタイミングを計算します。
        /// </summary>
        /// <param name="position">譜面位置</param>
        /// <returns>指定された位置のタイミング</returns>
        public long CalculateTimingFromPosition(Position position)
        {
            var tick = CalculateTickFromPosition(position);
            var measureIndex = position.MeasureIndex.Value;
            return CalculateTiming(measureIndex, tick);
        }

        /// <summary>
        /// 指定された位置のタイミングを計算します。
        /// </summary>
        /// <param name="measureIndex">対象小節番号</param>
        /// <param name="tick">ティック</param>
        /// <returns>指定された位置のタイミング</returns>
        public long CalculateTiming(int measureIndex, int tick)
        {
            var measureStartTiming = CalculateMeasureStartTiming(measureIndex);
            var measureDuration = CalculateMeasureDurationUpToTick(measureIndex, tick);
            return (long)(measureStartTiming + measureDuration);
        }

        /// <summary>
        /// 指定された小節が開始されるタイミングを取得します。
        /// </summary>
        /// <param name="measureIndex"></param>
        /// <returns>指定された小節が開始されるタイミング</returns>
        public float CalculateMeasureStartTiming(int measureIndex)
        {
            // 負の小節番号に対する既存の戻り値も維持する。
            if (measureIndex <= 0) return 0f;
            lock (cacheLock)
            {
                if (canCacheTimings && EnsureMeasureStartTiming(measureIndex)) return measureStartTimings[measureIndex];
            }
            return CalculateMeasureStartTimingWithoutCache(measureIndex);
        }

        private float CalculateMeasureStartTimingWithoutCache(int measureIndex)
        {
            var timingSum = 0f;
            for (var i = 0; i < measureIndex; i++)
            {
                timingSum += CalculateMeasureDuration(i);
            }
            return timingSum;
        }

        private bool EnsureMeasureStartTiming(int measureIndex)
        {
            while (measureStartTimings.Count <= measureIndex)
            {
                var previous = measureStartTimings[^1];
                var next = previous + CalculateMeasureDuration(measureStartTimings.Count - 1);
                if (!IsFiniteTiming(next) || next <= previous)
                {
                    canCacheTimings = false;
                    return false;
                }
                measureStartTimings.Add(next);
            }
            return true;
        }

        private bool EnsureMeasureBoundaryTiming(int measureIndex)
        {
            while (measureBoundaryTimings.Count <= measureIndex)
            {
                var index = measureBoundaryTimings.Count;
                if (!EnsureMeasureStartTiming(index)) return false;
                // CalculateTiming(index, 0) と同じ加算・整数化の境界を維持する。
                var timing = measureStartTimings[index] + CalculateMeasureDurationUpToTick(index, 0);
                if (!IsFiniteTiming(timing) || timing < 0f || timing >= (float)long.MaxValue)
                {
                    canCacheTimings = false;
                    return false;
                }
                var boundary = (long)timing;
                if (index > 0 && boundary < measureBoundaryTimings[index - 1])
                {
                    canCacheTimings = false;
                    return false;
                }
                measureBoundaryTimings.Add(boundary);
            }
            return true;
        }

        private static bool IsFiniteTiming(float timing)
        {
            return !float.IsNaN(timing) && !float.IsInfinity(timing);
        }

        /// <summary>
        /// 指定された小節の持続時間を求めます。
        /// </summary>
        /// <param name="measureIndex">対象小節</param>
        /// <returns>指定された小節の持続時間</returns>
        private float CalculateMeasureDuration(int measureIndex)
        {
            var maxTick = CalculateMeasureMaxTick(measureIndex);
            return CalculateMeasureDurationUpToTick(measureIndex, maxTick);
        }

        /// <summary>
        /// 指定された小節内の、指定されたティックまでの持続時間を求めます。
        /// </summary>
        /// <param name="measureIndex">対象小節</param>
        /// <param name="maxTick">最大ティック</param>
        /// <returns>指定された小節内での指定されたティックまでの持続時間</returns>
        private float CalculateMeasureDurationUpToTick(int measureIndex, int maxTick)
        {
            var measureMaxTick = CalculateMeasureMaxTick(measureIndex);
            if (maxTick > measureMaxTick) throw new ArgumentException();

            var measureLength = measureLengthProvider.GetMeasureLengthAt(measureIndex);
            var measureStartBpm = bpmProvider.GetMeasureStartBpm(measureIndex);
            var bpmChanges = bpmProvider.GetBpmChangeEventsAt(measureIndex);

            // BPMの変動がない場合はそのまま
            if (bpmChanges.Count == 0)
            {
                var measureMilliseconds = CalculateQuarterNoteMilliseconds(measureStartBpm.Value) * measureLength.GetQuarterNoteCount();
                return measureMilliseconds * ((float)maxTick / measureMaxTick);
            }

            var duration = 0f;
            for (var i = 0; i < bpmChanges.Count + 1; i++)
            {
                var bpm = i == 0 ? measureStartBpm : bpmChanges[i - 1].Bpm;
                var startTick = i == 0 ? 0 : bpmChanges[i - 1].Tick.Value;
                var endTick = i == bpmChanges.Count ? measureMaxTick : bpmChanges[i].Tick.Value;
                if (endTick > maxTick)
                {
                    endTick = maxTick;
                }

                var tickDuration = endTick - startTick;
                var applyingRatio = (float)tickDuration / measureMaxTick;
                duration += CalculateQuarterNoteMilliseconds(bpm.Value) * measureLength.GetQuarterNoteCount() * applyingRatio;

                if (endTick == maxTick) break;
            }
            return duration;
        }

        /// <summary>
        /// 指定された小節の最大ティックを求めます。
        /// </summary>
        /// <param name="measureIndex">対象小節</param>
        /// <returns>指定された小節の最大ティック</returns>
        public int CalculateMeasureMaxTick(int measureIndex)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(measureIndex);
            return (int)Math.Floor((float)(measureLength.GetBeatCount() * tpb.Value)); // 小数点以下切り捨て
        }

        /// <summary>
        /// 一拍の長さをミリ秒単位で求めます。
        /// </summary>
        /// <param name="bpm">BPM</param>
        /// <returns>一拍の長さ（ミリ秒）</returns>
        private float CalculateQuarterNoteMilliseconds(float bpm)
        {
            return 60f / bpm * 1000f;
        }

        /// <summary>
        /// 指定された位置のティックを求めます。
        /// </summary>
        /// <param name="position">対象位置</param>
        /// <returns>指定された位置のティック</returns>
        private int CalculateTickFromPosition(Position position)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(position.MeasureIndex.Value);
            return position.CalculateTickInt(measureLength, tpb);
        }

        /// <summary>
        /// タイミングが所属する小節番号を求めます。
        /// </summary>
        /// <param name="timing">対象タイミング</param>
        /// <returns>タイミングが所属する小節番号</returns>
        public int CalculateMeasureIndexFromTiming(long timing)
        {
            if (timing < 0) throw new ArgumentException(nameof(timing)); // 小節番号が負の値を取ることはないためエラー
            lock (cacheLock)
            {
                if (canCacheTimings && EnsureMeasureBoundaryTiming(0))
                {
                    while (measureBoundaryTimings[^1] <= timing)
                    {
                        if (!EnsureMeasureBoundaryTiming(measureBoundaryTimings.Count)) break;
                    }
                    if (canCacheTimings)
                    {
                        var lower = 0;
                        var upper = measureBoundaryTimings.Count;
                        while (lower < upper)
                        {
                            var middle = lower + (upper - lower) / 2;
                            if (measureBoundaryTimings[middle] <= timing) lower = middle + 1;
                            else upper = middle;
                        }
                        return lower - 1;
                    }
                }
            }
            return CalculateMeasureIndexFromTimingWithoutCache(timing);
        }

        private int CalculateMeasureIndexFromTimingWithoutCache(long timing)
        {
            var measureIndex = 0;
            while (true)
            {
                var measureStartTiming = (long)(CalculateMeasureStartTimingWithoutCache(measureIndex) + CalculateMeasureDurationUpToTick(measureIndex, 0));
                if (measureStartTiming > timing)
                {
                    measureIndex--;
                    break;
                }
                measureIndex++;
            }
            return measureIndex;
        }
    }
}
