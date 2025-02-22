using System;
using UOC.Analyze;
using UOC.Chart;
using UOC.Chart.Notes;

namespace UOC
{
    /// <summary>
    /// UOCの情報を持つオブジェクト
    /// </summary>
    public class UOCObject
    {
        private readonly ChartPropertyGroup chartPropertyGroup;
        private readonly NoteDefCollection noteDefCollection;
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NoteProfileCollection noteProfileCollection;
        private readonly NoteGroupProfileCollection noteGroupProfileCollection;

        private UOCObject(ChartPropertyGroup chartPropertyGroup, NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection, NoteGroupProfileCollection noteGroupProfileCollection)
        {
            this.chartPropertyGroup = chartPropertyGroup ?? throw new ArgumentNullException(nameof(chartPropertyGroup));
            this.noteDefCollection = noteDefCollection ?? throw new ArgumentNullException(nameof(noteDefCollection));
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.noteProfileCollection = noteProfileCollection ?? throw new ArgumentNullException(nameof(noteProfileCollection));
            this.noteGroupProfileCollection = noteGroupProfileCollection ?? throw new ArgumentNullException(nameof(noteGroupProfileCollection));
        }

        public static UOCObject Parse(UOCString uocString)
        {
            if (uocString == null) throw new ArgumentNullException(nameof(uocString));

            var lineCollection = UOCLineCollection.Parse(uocString);

            // PROPERTIESセクション行のパース
            var rawPropertiesSectionLines = lineCollection.GetLinesIn(SectionType.Properties);
            var propertiesSectionLineCollection = PropertiesSectionLineCollection.ParseUOCLines(rawPropertiesSectionLines);
            var propertyGroup = propertiesSectionLineCollection.CreateChartPropertyGroup();

            // NOTEDEFセクション行のパース
            var rawNotesDefsSectionLines = lineCollection.GetLinesIn(SectionType.NotesDefs);
            var notesDefsSectionLineCollection = NotesDefsSectionLineCollection.ParseUOCLines(rawNotesDefsSectionLines);
            var noteDefCollection = notesDefsSectionLineCollection.CreateNoteDefCollection();
            var noteGroupDefCollection = notesDefsSectionLineCollection.CreateNoteGroupDefCollection();

            // CHARTセクション行のパース
            var rawChartSectionLines = lineCollection.GetLinesIn(SectionType.Chart);
            var chartSectionLineCollection = ChartSectionLineCollection.ParseUOCLines(rawChartSectionLines);
            var noteProfileCollection = NoteProfileCollection.Create(noteDefCollection, chartSectionLineCollection);
            var noteGroupProfileCollection = NoteGroupProfileCollection.Create(noteGroupDefCollection, noteProfileCollection);

            return new UOCObject(propertyGroup, noteDefCollection, noteGroupDefCollection, noteProfileCollection, noteGroupProfileCollection);
        }

        /// <summary>
        /// 譜面プロパティグループ
        /// </summary>
        public ChartPropertyGroup ChartPropertyGroup => chartPropertyGroup;

        /// <summary>
        /// ノート定義コレクション
        /// </summary>
        public NoteDefCollection NoteDefCollection => noteDefCollection;

        /// <summary>
        /// ノートグループ定義コレクション
        /// </summary>
        public NoteGroupDefCollection NoteGroupDefCollection => noteGroupDefCollection;

        /// <summary>
        /// ノートプロファイルコレクション
        /// </summary>
        public NoteProfileCollection NoteProfileCollection => noteProfileCollection;

        /// <summary>
        /// ノートプロファイルコレクション
        /// </summary>
        public NoteGroupProfileCollection NoteGroupProfileCollection => noteGroupProfileCollection;
    }
}
