using System;
using System.Collections.Generic;
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
        private readonly BpmProvider bpmProvider;
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly MeasureLengthProvider measureLengthProvider;
        private readonly AnalysisSetting analysisSetting;
        private readonly Tpb tpb;
        private readonly long instantiateTiming;
        private readonly long enabledTiming;

        internal NotePlaybackProvider(NoteProfile noteProfile, EventProviders eventsProvider, AnalysisSetting analysisSetting, Tpb tpb, MeasureIndex maxMeasureIndex)
        {
            if (eventsProvider == null) throw new ArgumentNullException(nameof(eventsProvider));
            if (maxMeasureIndex == null) throw new ArgumentNullException(nameof(maxMeasureIndex));

            this.noteProfile = noteProfile ?? throw new ArgumentNullException(nameof(noteProfile));
            this.analysisSetting = analysisSetting ?? throw new ArgumentNullException(nameof(analysisSetting));
            this.tpb = tpb ?? throw new ArgumentNullException(nameof(tpb));

            bpmProvider = eventsProvider.BpmProvider;
            speedMultiplierProvider = eventsProvider.SpeedMultiplierProvider;
            measureLengthProvider = eventsProvider.MeasureLengthProvider;
            enabledTiming = CalculateTimingFromPosition(noteProfile.Position);
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
        public NoteGuid NoteGuid => noteProfile.NoteGuid;

        /*
         * ↓↓↓ 各種計算処理 ↓↓↓
         * なるべく新しいインスタンスを生成しないように実装
         * (軽量化の余地あり)
         */

        // TODO: 計算量が多いため、CalculateMeasureDuration()の処理結果をDictionaryでキャッシュしたい

        /// <summary>
        /// タイミングからノートの位置を求めます。
        /// ノート生成位置を1,判定位置を0とし、それ以降は負の値をとります。
        /// </summary>
        /// <param name="timing">タイミング</param>
        /// <returns>ノートの位置</returns>
        public float CalculateNotePosition(long timing)
        {
            var basicSpeed = analysisSetting.BasicSpeed;

            /* 小節線以降のハイスピを無視する処理 */
            if (analysisSetting.IgnoreSpeedChangesAfterJudgeLine && timing > enabledTiming)
            {
                return (enabledTiming - timing) / basicSpeed.MoveDuration;
            }

            /* 計算するタイミング範囲を求める */
            var startTiming = Math.Min(timing, enabledTiming);
            var endTiming = Math.Max(timing, enabledTiming);

            /* 小節範囲内のスピード変動をすべて求める */
            //var startMeasureIndex = CalculateMeasureIndexFromTiming(startTiming);
            //var endMeasureIndex = CalculateMeasureIndexFromTiming(endTiming);
            //var preAppliedSpeedMultiplier = speedMultiplierProvider.GetSpeedMultiplierAt(Math.Min(startMeasureIndex, 0), noteProfile.Layer);
            //var speedChangeEvents = speedMultiplierProvider.GetSpeedChangeEventsWithoutStartPoint(startMeasureIndex, endMeasureIndex, noteProfile.Layer);
            var preAppliedSpeedMultiplier = new SpeedMultiplier(1.0f);
            var speedChangeEvents = new List<SpeedMultiplierChangeEvent>();

            /* スピード変動がない場合はそのまま返す */
            if (speedChangeEvents.Count == 0)
            {
                return (enabledTiming - timing) * preAppliedSpeedMultiplier.Multiplier / basicSpeed.MoveDuration;
            }

            // 移動距離を計算
            var moveDist = 0f;
            for (int i = 0; i < speedChangeEvents.Count + 1; i++)
            {
                var speedMultiplier = i == 0 ? preAppliedSpeedMultiplier : speedChangeEvents[i - 1].SpeedMultiplier;
                var start = i == 0 ? startTiming : CalculateTiming(speedChangeEvents[i - 1].MeasureIndex.Value, speedChangeEvents[i - 1].Tick.Value);
                var end = i == speedChangeEvents.Count ? endTiming : CalculateTiming(speedChangeEvents[i].MeasureIndex.Value, speedChangeEvents[i].Tick.Value);
                moveDist += (end - start) * speedMultiplier.Multiplier;
            }

            //return moveDist / basicSpeed.MoveDuration * (timing > enabledTiming ? -1 : 1);
            return 0;




            //var basicSpeed = analysisSetting.BasicSpeed;

            ///* タイミングが負の場合、譜面始点のスピード倍率を適用する */
            //if (timing < 0)
            //{
            //    var initialSpeedMultiplier = speedMultiplierProvider.GetSpeedMultiplierAt(0, noteProfile.Layer);
            //    return timing * initialSpeedMultiplier.Multiplier / basicSpeed.MoveDuration;
            //}

            ///* 小節線以降のハイスピを無視する処理 */
            //if (analysisSetting.IgnoreSpeedChangesAfterJudgeLine && timing > enabledTiming)
            //{
            //    return (enabledTiming - timing) / basicSpeed.MoveDuration;
            //}

            ///* 計算するタイミング範囲を求める */
            //var startTiming = Math.Min(timing, enabledTiming);
            //var endTiming = Math.Max(timing, enabledTiming);

            ///* 小節範囲内のスピード変動をすべて求める */
            //var startMeasureIndex = CalculateMeasureIndexFromTiming(startTiming);
            //var endMeasureIndex = CalculateMeasureIndexFromTiming(endTiming);
            //var preAppliedSpeedMultiplier = speedMultiplierProvider.GetSpeedMultiplierAt(startMeasureIndex, noteProfile.Layer);
            //var speedChangeEvents = speedMultiplierProvider.GetSpeedChangeEventsWithoutStartPoint(startMeasureIndex, endMeasureIndex, noteProfile.Layer);

            ///* スピード変動がない場合はそのまま返す */
            //if (speedChangeEvents.Count == 0)
            //{
            //    return (timing - enabledTiming) * preAppliedSpeedMultiplier.Multiplier / basicSpeed.MoveDuration;
            //}

            //// 移動距離を計算
            //var moveDist = 0f;
            //for (int i = 0; i < speedChangeEvents.Count + 1; i++)
            //{
            //    var speedMultiplier = i == 0 ? preAppliedSpeedMultiplier : speedChangeEvents[i - 1].SpeedMultiplier;
            //    var start = i == 0 ? startTiming : CalculateTiming(speedChangeEvents[i - 1].MeasureIndex.Value, speedChangeEvents[i - 1].Tick.Value);
            //    var end = i == speedChangeEvents.Count ? endTiming : CalculateTiming(speedChangeEvents[i].MeasureIndex.Value, speedChangeEvents[i].Tick.Value);
            //    moveDist += (end - start) * speedMultiplier.Multiplier;
            //}

            //return moveDist / basicSpeed.MoveDuration * (timing > enabledTiming ? -1 : 1);
        }

        /// <summary>
        /// 指定された位置からタイミングを計算します。
        /// </summary>
        /// <param name="position">譜面位置</param>
        /// <returns>指定された位置のタイミング</returns>
        private long CalculateTimingFromPosition(Position position)
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
        private long CalculateTiming(int measureIndex, int tick)
        {
            var measureStartTiming = CalculateMeasureStartTiming(measureIndex);
            var measureDuration = CalculateMeasureDurationUpToTick(measureIndex, tick);
            Console.Write($"[{measureDuration}]");
            return (long)(measureStartTiming + measureDuration);
        }

        /// <summary>
        /// 指定された小節が開始されるタイミングを取得します。
        /// </summary>
        /// <param name="measureIndex"></param>
        /// <returns>指定された小節が開始されるタイミング</returns>
        private float CalculateMeasureStartTiming(int measureIndex)
        {
            float timingSum = 0;
            for (int i = 0; i < measureIndex; i++)
            {
                timingSum += CalculateMeasureDuration(i);
            }
            return timingSum;
        }

        /// <summary>
        /// 指定された小節の持続時間を求めます。
        /// </summary>
        /// <param name="measureIndex">対象小節</param>
        /// <returns>指定された小節の持続時間</returns>
        private float CalculateMeasureDuration(int measureIndex)
        {
            int maxTick = CalculateMeasureMaxTick(measureIndex);
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

            float duration = 0;
            for (int i = 0; i < bpmChanges.Count + 1; i++)
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
        private int CalculateMeasureMaxTick(int measureIndex)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(measureIndex);
            return (int)Math.Floor((float)(measureLength.GetBeatCount() * tpb.Value)); // 小数点以下切り捨て
        }

        /// <summary>
        /// 一拍の長さをミリ秒単位で求めます。
        /// </summary>
        /// <param bpm="bpm">BPM</param>
        /// <returns>一拍の長さ（ミリ秒）</returns>
        private float CalculateQuarterNoteMilliseconds(float bpm)
        {
            return 60f / bpm * 1000f;
        }

        /// <summary>
        /// 指定された位置のティックを求めます。
        /// </summary>
        /// <param bpm="position">対象位置</param>
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
        /// <returns>タイミングが所属する小節番号returns>
        private int CalculateMeasureIndexFromTiming(long timing)
        {
            if (timing < 0) throw new ArgumentException(nameof(timing)); // 小節番号が負の値を取ることはないためエラー
            int measureIndex = 0;
            while (true)
            {
                float measureStartTiming = CalculateMeasureStartTiming(measureIndex);
                if (measureStartTiming > timing)
                {
                    measureIndex--;
                    break;
                }
                measureIndex++;
            }
            return measureIndex;
        }

        private long GetInstantiateTiming(AnalysisSetting analysisSetting, MeasureIndex maxMeasureIndex)
        {
            var minimumTiming = analysisSetting.MinimumTiming;
            var interval = analysisSetting.NotesInstantiationInterval;
            var maxTiming = (long)CalculateMeasureStartTiming(maxMeasureIndex.Value + 1); // 最大のタイミングを、ノートが存在する最大小節の次の小節の開始点とする。
            for (long i = minimumTiming; i < maxTiming; i += interval)
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
