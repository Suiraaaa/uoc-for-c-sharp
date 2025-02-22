using System;
using UOC.Analyze;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// ノートの再生を提供するクラス
    /// </summary>
    public class NotePlaybackProvider
    {
        private const string KEY_X = "x";
        private const string KEY_SIZE = "size";

        private readonly NoteProfile noteProfile;
        private readonly BPMProvider bpmProvider;
        private readonly SpeedMultiplierProvider SpeedMultiplierProvider;
        private readonly MeasureLengthProvider measureLengthProvider;
        private readonly AnalysisSetting analysisSetting;
        private readonly TPB tpb;
        private readonly long instantiateTiming;
        private readonly long enabledTiming;

        internal NotePlaybackProvider(NoteProfile noteProfile, EventsProvider eventsProvider, AnalysisSetting analysisSetting, TPB tpb, MeasureIndex maxMeasureIndex)
        {
            if (eventsProvider == null) throw new ArgumentNullException(nameof(eventsProvider));
            if (maxMeasureIndex == null) throw new ArgumentNullException(nameof(maxMeasureIndex));

            this.noteProfile = noteProfile ?? throw new ArgumentNullException(nameof(noteProfile));
            this.analysisSetting = analysisSetting ?? throw new ArgumentNullException(nameof(analysisSetting));
            this.tpb = tpb ?? throw new ArgumentNullException(nameof(tpb));

            bpmProvider = eventsProvider.BPMProvider;
            SpeedMultiplierProvider = eventsProvider.SpeedMultiplierProvider;
            measureLengthProvider = eventsProvider.MeasureLengthProvider;
            instantiateTiming = GetInstantiateTiming(analysisSetting, maxMeasureIndex);
            enabledTiming = CalculateTimingFromPosition(noteProfile.Position);
        }

        /// <summary>
        /// ノートID
        /// </summary>
        public string NoteId => noteProfile.NoteDef.NoteId.Value;

        /// <summary>
        /// ノートプロパティ
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
        public string Guid => noteProfile.Guid.Value;

        /// <summary>
        /// ノートの水平位置(PropertyKey:x)を取得します。
        /// </summary>
        /// <returns>ノートの水平位置</returns>
        public int GetHorizontalPosition()
        {
            var propertyGroup = noteProfile.PropertyGroup;
            if (!propertyGroup.HasKey(KEY_X))
            {
                throw new InvalidOperationException($"ノートは水平位置情報(x)を持っていません。(ノートID: {noteProfile.NoteDef.NoteId.Value})");
            }
            return propertyGroup.GetPropertyByKey(KEY_X).Value.AsInt();
        }

        /// <summary>
        /// ノートのサイズ(PropertyKey:size)を取得します。
        /// </summary>
        /// <returns>ノートのサイズ</returns>
        public int GetSize()
        {
            var propertyGroup = noteProfile.PropertyGroup;
            if (!propertyGroup.HasKey(KEY_SIZE))
            {
                throw new InvalidOperationException($"ノートはサイズ情報(size)を持っていません。(ノートID: {noteProfile.NoteDef.NoteId.Value})");
            }
            return propertyGroup.GetPropertyByKey(KEY_SIZE).Value.AsInt();
        }

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

            /* タイミングが負の場合、譜面始点のスピード倍率を適用する */
            if (timing < 0)
            {
                var initialSpeedMultiplier = SpeedMultiplierProvider.GetSpeedMultiplierAt(0, noteProfile.Layer);
                return timing * initialSpeedMultiplier.Multiplier / basicSpeed.MoveDuration;
            }

            /* 小節線以降のハイスピを無視する処理 */
            if (analysisSetting.IgnoreSpeedChangesAfterJudgeLine && timing > enabledTiming)
            {
                return (enabledTiming - timing) / basicSpeed.GetMoveDurationAsMilliseconds();
            }

            /* 計算するタイミング範囲を求める */
            var startTiming = timing <= enabledTiming ? timing : enabledTiming;
            var endTiming = timing <= enabledTiming ? enabledTiming : timing;

            /* 小節範囲内のスピード変動をすべて求める */
            var startMeasureIndex = CalculateMeasureIndexFromTiming(startTiming);
            var endMeasureIndex = CalculateMeasureIndexFromTiming(endTiming);
            var preAppliedSpeedMultiplier = SpeedMultiplierProvider.GetSpeedMultiplierAt(startMeasureIndex, noteProfile.Layer);
            var speedChangeEvents = SpeedMultiplierProvider.GetSpeedChangeEventsWithoutStartPoint(startMeasureIndex, endMeasureIndex, noteProfile.Layer);

            /* スピード変動がない場合はそのまま返す */
            if (speedChangeEvents.Count == 0)
            {
                return timing * preAppliedSpeedMultiplier.Multiplier / basicSpeed.MoveDuration;
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

            return moveDist / basicSpeed.GetMoveDurationAsMilliseconds() * (timing > enabledTiming ? -1 : 1);
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
            return CalculateTiming(tick, measureIndex);
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
            var measureStartBPM = bpmProvider.GetMeasureStartBPM(measureIndex);
            var bpmChanges = bpmProvider.GetBPMChangeEventsAt(measureIndex);

            // BPMの変動がない場合はそのまま
            if (bpmChanges.Count == 0)
            {
                return CalculateBeatMilliSeconds(measureStartBPM.Value) * measureLength.BeatsCount;
            }

            float duration = 0;
            bool completed = false;
            for (int i = 0; i < bpmChanges.Count + 1; i++)
            {
                int startTick = i == 0 ? 0 : bpmChanges[i - 1].Tick.Value;
                int endTick = bpmChanges[i].Tick.Value;
                if (endTick > maxTick)
                {
                    endTick = maxTick;
                    completed = true;
                }

                int tickDuration = endTick - startTick;
                float applyingRatio = (float)tickDuration / measureMaxTick;
                duration += CalculateBeatMilliSeconds(measureStartBPM.Value) * measureLength.BeatsCount * applyingRatio;

                if (completed) break;
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
            return (int)Math.Floor(measureLength.BeatsCount * tpb.Value); // 小数点以下切り捨て
        }

        /// <summary>
        /// 一拍の長さをミリ秒単位で求めます。
        /// </summary>
        /// <param bpm="bpm">BPM</param>
        /// <returns>一拍の長さ（ミリ秒）</returns>
        private float CalculateBeatMilliSeconds(float bpm)
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
            int measureInex = 0;
            while (true)
            {
                float measureStartTiming = CalculateMeasureStartTiming(measureInex);
                if (measureStartTiming > timing)
                {
                    measureInex--;
                    break;
                }
                measureInex++;
            }
            return measureInex;
        }

        private long GetInstantiateTiming(AnalysisSetting analysisSetting, MeasureIndex maxMeasureIndex)
        {
            var minTiming = analysisSetting.MinTiming;
            var interval = analysisSetting.NotesInstantiateTimingInterval;
            var maxTiming = (long)CalculateMeasureStartTiming(maxMeasureIndex.Value + 1); // 最大のタイミングを、ノートが存在する最大小節の次の小節の開始点とする。
            for (long i = minTiming; i < maxTiming; i += interval)
            {
                var position = CalculateNotePosition(i);
                if (position < 1f)
                {
                    return i - interval;
                }
            }
            return maxTiming;
        }
    }
}
