using System;
using Uoc.Analyze;
using Uoc.Chart;
using Uoc.Chart.Notes;

namespace Uoc
{
    /// <summary>
    /// UOCの情報を持つオブジェクト
    /// </summary>
    public class UocObject
    {
        private readonly ChartPropertyGroup chartPropertyGroup;
        private readonly NoteDefCollection noteDefCollection;
        private readonly NoteGroupDefCollection noteGroupDefCollection;
        private readonly NoteProfileCollection noteProfileCollection;
        private readonly NoteGroupProfileCollection noteGroupProfileCollection;

        private UocObject(ChartPropertyGroup chartPropertyGroup, NoteDefCollection noteDefCollection, NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection, NoteGroupProfileCollection noteGroupProfileCollection)
        {
            this.chartPropertyGroup = chartPropertyGroup ?? throw new ArgumentNullException(nameof(chartPropertyGroup));
            this.noteDefCollection = noteDefCollection ?? throw new ArgumentNullException(nameof(noteDefCollection));
            this.noteGroupDefCollection = noteGroupDefCollection ?? throw new ArgumentNullException(nameof(noteGroupDefCollection));
            this.noteProfileCollection = noteProfileCollection ?? throw new ArgumentNullException(nameof(noteProfileCollection));
            this.noteGroupProfileCollection = noteGroupProfileCollection ?? throw new ArgumentNullException(nameof(noteGroupProfileCollection));
        }

        public static UocObject Parse(UocString uocString)
        {
            if (uocString == null) throw new ArgumentNullException(nameof(uocString));

            var lineCollection = UocLineCollection.Parse(uocString);

            // PROPERTIESセクション行のパース
            var rawPropertiesSectionLines = lineCollection.GetLinesIn(SectionType.Properties);
            var propertiesSectionLineCollection = PropertiesSectionLineCollection.ParseUocLines(rawPropertiesSectionLines);
            var propertyGroup = propertiesSectionLineCollection.CreateChartPropertyGroup();

            // NOTEDEFセクション行のパース
            var rawNotesDefsSectionLines = lineCollection.GetLinesIn(SectionType.NotesDefs);
            var notesDefsSectionLineCollection = NotesDefsSectionLineCollection.ParseUocLines(rawNotesDefsSectionLines);
            var noteDefCollection = notesDefsSectionLineCollection.CreateNoteDefCollection();
            var noteGroupDefCollection = notesDefsSectionLineCollection.CreateNoteGroupDefCollection();

            // CHARTセクション行のパース
            var rawChartSectionLines = lineCollection.GetLinesIn(SectionType.Chart);
            var chartSectionLineCollection = ChartSectionLineCollection.ParseUocLines(rawChartSectionLines);
            var noteProfileCollection = NoteProfileCollection.Create(noteDefCollection, chartSectionLineCollection);
            var noteGroupProfileCollection = NoteGroupProfileCollection.Create(noteGroupDefCollection, noteProfileCollection);

            return new UocObject(propertyGroup, noteDefCollection, noteGroupDefCollection, noteProfileCollection, noteGroupProfileCollection);
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
