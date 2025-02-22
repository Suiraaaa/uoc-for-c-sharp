using System;
using System.Collections.Generic;
using System.Linq;
using UOC.Analyze;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// ノートプロファイルコレクション
    /// </summary>
    public class NoteProfileCollection
    {
        private readonly IReadOnlyList<NoteProfile> noteProfiles;

        public NoteProfileCollection()
        {
            noteProfiles = new List<NoteProfile>();
        }

        public NoteProfileCollection(IReadOnlyList<NoteProfile> noteProfiles)
        {
            if (noteProfiles == null) throw new ArgumentNullException(nameof(noteProfiles));
            this.noteProfiles = noteProfiles.OrderBy(x => x.Position).ToList(); // 位置で昇順にソート
        }

        /// <summary>
        /// 必要最低限の情報のみを含むノートプロファイルコレクションを作成します。
        /// </summary>
        /// <param name="measureLength">初期小節長</param>
        /// <param name="bpm">初期BPM</param>
        /// <param name="speedMultiplier">初期スピード倍率（推奨値：1.0）</param>
        /// <param name="layer">これらの情報を配置するレイヤー</param>
        /// <returns>必要最低限の情報のみを含むノートプロファイルコレクション</returns>
        public static NoteProfileCollection CreateMinimum(MeasureLength measureLength, BPM bpm, SpeedMultiplier speedMultiplier, Layer layer)
        {
            var noteProfiles = new List<NoteProfile>()
            {
                new NoteProfile(
                    noteDef: NoteDef.MeasureLine,
                    position: Position.ChartStart,
                    propertyValues: new List<string>() { },
                    layer: layer,
                    channel: Channel.Empty
                ),
                new NoteProfile(
                    noteDef: NoteDef.BPMChange,
                    position: Position.ChartStart,
                    propertyValues: new List<string>() { bpm.Value.ToString() },
                    layer: layer,
                    channel: Channel.Empty
                ),
                new NoteProfile(
                    noteDef: NoteDef.SpeedChange,
                    position: Position.ChartStart,
                    propertyValues: new List<string>() { speedMultiplier.Multiplier.ToString() },
                    layer: layer,
                    channel: Channel.Empty
                ),
                new NoteProfile(
                    noteDef: NoteDef.MeasureLengthChange,
                    position: Position.ChartStart,
                    propertyValues: new List<string>() { measureLength.Denominator.ToString(), measureLength.Numerator.ToString() },
                    layer: layer,
                    channel: Channel.Empty
                ),
            };
            return new NoteProfileCollection(noteProfiles);
        }

        internal static NoteProfileCollection Create(NoteDefCollection noteDefCollection, ChartSectionLineCollection chartSectionLineCollection)
        {
            var noteProfiles = new List<NoteProfile>();
            foreach (var line in chartSectionLineCollection.ChartSectionLines)
            {
                var adding = line.ToNoteProfiles(noteDefCollection);
                noteProfiles = noteProfiles.Concat(adding).ToList();
            }
            return new NoteProfileCollection(noteProfiles);
        }

        /// <summary>
        /// ノートプロファイルのリスト（位置で昇順）
        /// </summary>
        public IReadOnlyList<NoteProfile> NoteProfiles => noteProfiles;

        public NoteProfile GetNoteProfileByGuid(NoteGuid guid)
        {
            return noteProfiles.FirstOrDefault(x => x.Guid == guid) ?? throw new KeyNotFoundException($"Guidが\"{guid}\"のノートは見つかりませんでした。");
        }

        /// <summary>
        /// ノートを追加もしくは置換します。
        /// 同一なGuidを持つノートがすでに存在する場合は置換します。
        /// </summary>
        /// <param name="putNoteProfile">追加するノート</param>
        /// <returns>操作後のNoteProfileCollection</returns>
        public NoteProfileCollection PutOrReplace(NoteProfile putNoteProfile)
        {
            var editingNoteProfiles = new List<NoteProfile>(noteProfiles);
            var sameGuidNoteIndex = editingNoteProfiles.FindIndex(x => x.Guid == putNoteProfile.Guid);
            if (sameGuidNoteIndex != -1)
            {
                editingNoteProfiles[sameGuidNoteIndex] = putNoteProfile;
            }
            else
            {
                editingNoteProfiles.Add(putNoteProfile);
            }
            return new NoteProfileCollection(editingNoteProfiles);
        }

        /// <summary>
        /// ノートを追加します。
        /// 同一なGuidを持つノートがすでに存在する場合は置換します。
        /// </summary>
        /// <param name="putNoteProfiles">追加するノートリスト</param>
        /// <returns>操作後のNoteProfileCollection</returns>
        public NoteProfileCollection PutOrReplace(IReadOnlyList<NoteProfile> putNoteProfiles)
        {
            var editingNoteProfiles = new List<NoteProfile>(noteProfiles);
            foreach (var putNoteProfile in putNoteProfiles)
            {
                var sameGuidNoteIndex = editingNoteProfiles.FindIndex(x => x.Guid == putNoteProfile.Guid);
                if (sameGuidNoteIndex != -1)
                {
                    editingNoteProfiles[sameGuidNoteIndex] = putNoteProfile;
                }
                else
                {
                    editingNoteProfiles.Add(putNoteProfile);
                }
            }
            return new NoteProfileCollection(editingNoteProfiles);
        }

        /// <summary>
        /// 指定されたGUIDを持つノートを削除します。
        /// </summary>
        /// <param name="guid">削除対象ノートのGUID</param>
        /// <returns>操作後のNoteProfileCollection</returns>
        public NoteProfileCollection Remove(NoteGuid guid)
        {
            var editingNoteProfiles = new List<NoteProfile>(noteProfiles);
            editingNoteProfiles.RemoveAll(x => x.Guid == guid);
            return new NoteProfileCollection(editingNoteProfiles);
        }

        /// <summary>
        /// 指定されたGUIDを持つノートを削除します。
        /// </summary>
        /// <param name="guids">削除対象ノートのGUIDリスト</param>
        /// <returns>操作後のNoteProfileCollection</returns>
        public NoteProfileCollection Remove(IReadOnlyList<NoteGuid> guids)
        {
            var editingNoteProfiles = new List<NoteProfile>(noteProfiles);
            foreach (var guid in guids)
            {
                editingNoteProfiles.RemoveAll(x => x.Guid == guid);
            }
            return new NoteProfileCollection(editingNoteProfiles);
        }

        public MeasureIndex GetMaxMeasureIndex()
        {
            int max = noteProfiles.Max(x => x.Position.MeasureIndex.Value);
            return noteProfiles.First(x => x.Position.MeasureIndex.Value == max).Position.MeasureIndex;
        }

        public EventsProvider CreateEventsProvider(TPB tpb)
        {
            var measureLengthProvider = CreateMeasureLengthProvider();
            var bpmProvider = CreateBPMProvider(measureLengthProvider, tpb);
            var speedMultiplierProvider = CreateSpeedMultiplierProvider(measureLengthProvider, tpb);
            return new EventsProvider(bpmProvider, speedMultiplierProvider, measureLengthProvider);
        }

        public MeasureLengthProvider CreateMeasureLengthProvider()
        {
            var measureLengthChangeNotes = new List<NoteProfile>();
            foreach (var noteProfile in noteProfiles)
            {
                if (!noteProfile.IsMeasureLengthChangeNote()) continue;
                measureLengthChangeNotes.Add(noteProfile);
            }
            var measureLengthEvents = measureLengthChangeNotes.Select(x => x.ToMeasureLengthChangeEvent()).ToList();
            return new MeasureLengthProvider(measureLengthEvents);
        }

        public BPMProvider CreateBPMProvider(MeasureLengthProvider measureLengthProvider, TPB tpb)
        {
            var bpmChangeNotes = new List<NoteProfile>();
            foreach (var noteProfile in noteProfiles)
            {
                if (!noteProfile.IsBPMChangeNote()) continue;
                bpmChangeNotes.Add(noteProfile);
            }
            var bpmChangeEvents = bpmChangeNotes.Select(x => x.ToBPMChangeEvent(measureLengthProvider, tpb)).ToList();
            return new BPMProvider(bpmChangeEvents);
        }

        public SpeedMultiplierProvider CreateSpeedMultiplierProvider(MeasureLengthProvider measureLengthProvider, TPB tpb)
        {
            var speedChangeNotes = new List<NoteProfile>();
            foreach (var noteProfile in noteProfiles)
            {
                if (!noteProfile.IsSpeedChangeNote()) continue;
                speedChangeNotes.Add(noteProfile);
            }
            var speedChangeEvents = speedChangeNotes.Select(x => x.ToSpeedChangeEvent(measureLengthProvider, tpb)).ToList();
            return new SpeedMultiplierProvider(speedChangeEvents);
        }
    }
}
