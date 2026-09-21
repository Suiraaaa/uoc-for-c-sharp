using System;
using Uoc.Analyze.Speed;
using Uoc.Chart;
using Uoc.Chart.Event;
using Uoc.Chart.Notes;
using Uoc.Chart.Property;

namespace Uoc.Analyze.Playback
{
    /// <summary>
    /// 単体ノートの再生に関する情報を提供するクラス
    /// </summary>
    public class NotePlaybackProvider
    {
        private readonly NoteProfile noteProfile;
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly AnalysisSetting analysisSetting;
        private readonly PlaybackTimingCalculator timingCalculator;
        private readonly long instantiateTiming;
        private readonly long enabledTiming;

        internal NotePlaybackProvider(NoteProfile noteProfile, SpeedMultiplierProvider speedMultiplierProvider, AnalysisSetting analysisSetting, PlaybackTimingCalculator timingCalculator, MeasureIndex maxMeasureIndex)
        {
            if (maxMeasureIndex == null) throw new ArgumentNullException(nameof(maxMeasureIndex));

            this.noteProfile = noteProfile ?? throw new ArgumentNullException(nameof(noteProfile));
            this.analysisSetting = analysisSetting ?? throw new ArgumentNullException(nameof(analysisSetting));
            this.timingCalculator = timingCalculator ?? throw new ArgumentNullException(nameof(timingCalculator));

            this.speedMultiplierProvider = speedMultiplierProvider ?? throw new ArgumentNullException(nameof(speedMultiplierProvider));
            enabledTiming = timingCalculator.CalculateTimingFromPosition(noteProfile.Position);
            instantiateTiming = GetInstantiateTiming(analysisSetting, maxMeasureIndex);
        }

        /// <summary>
        /// ノートID
        /// </summary>
        public NoteId NoteId => noteProfile.NoteDef.NoteId;

        /// <summary>
        /// ノートが持つプロパティ
        /// </summary>
        public PropertyGroup NoteProperties => noteProfile.PropertyGroup;

        /// <summary>
        /// ノートの生成タイミング
        /// </summary>
        public long InstantiateTiming => instantiateTiming;

        /// <summary>
        /// ノートの有効タイミング
        /// </summary>
        public long EnabledTiming => enabledTiming;

        /// <summary>
        /// ノートのGuid
        /// </summary>
        public Guid Guid => noteProfile.Guid;

        /*
         * ↓↓↓ 各種計算処理 ↓↓↓
         * なるべく新しいインスタンスを生成しないように実装
         * (軽量化の余地あり)
         */

        /// <summary>
        /// タイミングからノートの位置を求めます。
        /// ノート生成位置を1、判定位置を0とします。
        /// 速度倍率が正の場合、判定位置を通過した後は負の値をとります。
        /// </summary>
        /// <param name="timing">タイミング</param>
        /// <returns>ノートの位置</returns>
        public float CalculateNotePosition(long timing)
        {
            var basicSpeed = analysisSetting.BasicSpeed;

            /* 小節線以降のハイスピを無視する処理 */
            if (analysisSetting.IgnoreSpeedChangesAfterJudgeLine && timing > enabledTiming)
            {
                return (float)(((double)enabledTiming - timing) / basicSpeed.MoveDuration);
            }

            var startTiming = Math.Min(timing, enabledTiming);
            var endTiming = Math.Max(timing, enabledTiming);
            var moveDistance = CalculateMoveDistance(startTiming, endTiming);
            var direction = timing > enabledTiming ? -1 : 1;
            return (float)(moveDistance / basicSpeed.MoveDuration * direction);
        }

        private double CalculateMoveDistance(long startTiming, long endTiming)
        {
            if (startTiming == endTiming)
            {
                return 0;
            }

            var currentTiming = startTiming;
            var currentSpeedMultiplier = GetSpeedMultiplierAt(startTiming);
            var firstMeasureIndex = timingCalculator.CalculateMeasureIndexFromTiming(Math.Max(startTiming, 0));
            var lastMeasureIndex = timingCalculator.CalculateMeasureIndexFromTiming(Math.Max(endTiming, 0));
            var speedChangeEvents = speedMultiplierProvider.GetSpeedMultiplierChangeEventsAt(firstMeasureIndex, lastMeasureIndex, noteProfile.Layer);

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

        private SpeedMultiplier GetSpeedMultiplierAt(long timing)
        {
            if (timing < 0)
            {
                return speedMultiplierProvider.GetMeasureStartSpeedMultiplier(0, noteProfile.Layer);
            }

            var measureIndex = timingCalculator.CalculateMeasureIndexFromTiming(timing);
            var speedMultiplier = speedMultiplierProvider.GetMeasureStartSpeedMultiplier(measureIndex, noteProfile.Layer);
            var speedChangeEvents = speedMultiplierProvider.GetSpeedMultiplierChangeEventsAt(measureIndex, measureIndex, noteProfile.Layer);
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

        private long GetInstantiateTiming(AnalysisSetting analysisSetting, MeasureIndex maxMeasureIndex)
        {
            var minimumTiming = analysisSetting.MinimumTiming;
            var interval = analysisSetting.NotesInstantiationInterval;
            var maxTiming = (long)timingCalculator.CalculateMeasureStartTiming(maxMeasureIndex.Value + 1); // 最大のタイミングを、ノートが存在する最大小節の次の小節の開始点とする。
            for (var i = minimumTiming; i < maxTiming; i += interval)
            {
                var position = CalculateNotePosition(i);
                if (position <= 1f) // 位置が生成位置に到達した場合
                {
                    return Math.Max(i - interval, minimumTiming);
                }
            }
            return maxTiming;
        }
    }
}
