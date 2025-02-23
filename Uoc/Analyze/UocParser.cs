using System;
using Uoc.Chart.Notes;

namespace Uoc.Analyze
{
    /// <summary>
    /// UOC文字列のパースを行う静的クラス
    /// </summary>
    public static class UocParser
    {
        /// <summary>
        /// UOC文字列をパースし、UocObjectを作成します。
        /// </summary>
        /// <param name="uocString">パース対象のUOC文字列</param>
        /// <returns>作成されたUocObject</returns>
        /// <exception cref="ArgumentNullException">uocStringがnullの場合にスローされます。</exception>
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
    }
}
