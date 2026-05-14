using System;
using System.Collections.Generic;
using System.Linq;
using Uoc.Analyze.Speed;
using Uoc.Chart.Event;
using Uoc.Chart.Notes.Definition;
using Uoc.Parse.Line.ChartSection;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートプロファイルコレクション
    /// </summary>
    public class NoteProfileCollection
    {
        private readonly IReadOnlyList<NoteProfile> noteProfiles;

        private NoteGroupProfileCollection? noteGroupProfileCollection;

        internal NoteProfileCollection(IReadOnlyList<NoteProfile> noteProfiles)
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
        public static NoteProfileCollection CreateMinimum(MeasureLength measureLength, Bpm bpm, SpeedMultiplier speedMultiplier, Layer layer)
        {
            var noteProfiles = new List<NoteProfile>()
            {
                new NoteProfile(
                    noteDef: NoteDef.BpmChange,
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

        /// <summary>
        /// 保持するノーツ情報からNoteGroupProfileCollectionを作成します。
        /// </summary>
        /// <remarks>
        /// 一度作成されたNoteGroupProfileCollectionはキャッシュされ、以降は同一のインスタンスを返します。
        /// NoteGroupProfileCollectionは作成するたびに保持するGuidが変わるため、
        /// 常に同一のGuidを持つインスタンスを返すことを目的としています。
        /// </remarks>
        /// <param name="noteGroupDefCollection">ノート定義コレクション</param>
        /// <returns>作成もしくはキャッシュされたNoteGroupProfileCollection</returns>
        public NoteGroupProfileCollection CreateNoteGroupProfileCollection(NoteGroupDefCollection noteGroupDefCollection)
        {
            if (noteGroupProfileCollection is null)
            {
                noteGroupProfileCollection = NoteGroupProfileCollection.Create(noteGroupDefCollection, this);
            }
            return noteGroupProfileCollection;
        }

        public NoteProfile? GetNoteProfileByGuid(Guid guid)
        {
            return noteProfiles.FirstOrDefault(x => x.Guid == guid);
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
        public NoteProfileCollection Remove(Guid guid)
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
        public NoteProfileCollection Remove(IReadOnlyList<Guid> guids)
        {
            var editingNoteProfiles = new List<NoteProfile>(noteProfiles);
            foreach (var guid in guids)
            {
                editingNoteProfiles.RemoveAll(x => x.Guid == guid);
            }
            return new NoteProfileCollection(editingNoteProfiles);
        }

        /// <summary>
        /// 小節長に変更があった際に、絶対的な位置が変動しないようノーツの位置を再計算します。
        /// </summary>
        /// <param name="oldMeasureLengthProvider">変更前の小節長プロバイダ</param>
        /// <param name="newMeasureLengthProvider">変更後の小節長プロバイダ</param>
        /// <returns>再計算されたPosition</returns>
        public NoteProfileCollection RecalculateNotePositions(MeasureLengthProvider oldMeasureLengthProvider, MeasureLengthProvider newMeasureLengthProvider)
        {
            var currentNoteProfiles = new List<NoteProfile>(noteProfiles);
            var recalculateNotes = new List<NoteProfile>();
            foreach (var note in currentNoteProfiles)
            {
                var recalculatePosition = note.Position.RecalculatePosition(oldMeasureLengthProvider, newMeasureLengthProvider);
                recalculateNotes.Add(note.UpdatePosition(recalculatePosition));
            }
            return new NoteProfileCollection(recalculateNotes);
        }

        public MeasureIndex GetMaxMeasureIndex()
        {
            int max = noteProfiles.Max(x => x.Position.MeasureIndex.Value);
            return noteProfiles.First(x => x.Position.MeasureIndex.Value == max).Position.MeasureIndex;
        }

        public EventProviders CreateEventProviders(Tpb tpb)
        {
            var measureLengthProvider = CreateMeasureLengthProvider();
            var bpmProvider = CreateBpmProvider(measureLengthProvider, tpb);
            var speedMultiplierProvider = CreateSpeedMultiplierProvider(measureLengthProvider, tpb);
            return new EventProviders(bpmProvider, speedMultiplierProvider, measureLengthProvider);
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

        public BpmProvider CreateBpmProvider(MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            var bpmChangeNotes = new List<NoteProfile>();
            foreach (var noteProfile in noteProfiles)
            {
                if (!noteProfile.IsBpmChangeNote()) continue;
                bpmChangeNotes.Add(noteProfile);
            }
            var bpmChangeEvents = bpmChangeNotes.Select(x => x.ToBpmChangeEvent(measureLengthProvider, tpb)).ToList();
            return new BpmProvider(bpmChangeEvents);
        }

        public SpeedMultiplierProvider CreateSpeedMultiplierProvider(MeasureLengthProvider measureLengthProvider, Tpb tpb)
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
