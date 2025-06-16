using System;
using Uoc.Analyze;
using Uoc.Chart;

namespace Uoc
{
    /// <summary>
    /// UOCデータ
    /// </summary>
    public readonly struct UocData
    {
        private readonly ChartPropertyGroup chartProperties;
        private readonly NotesData chartData;

        public UocData(ChartPropertyGroup chartProperties, NotesData chartData)
        {
            this.chartProperties = chartProperties ?? throw new ArgumentNullException(nameof(chartProperties));
            this.chartData = chartData ?? throw new ArgumentNullException(nameof(chartData));
        }

        public static UocData Create(UocString uocString, AnalysisSetting analysisSetting)
        {
            var uocObject = UocParser.Parse(uocString);
            var chartPropertyGroup = uocObject.ChartPropertyGroup;
            var noteDefCollection = uocObject.NoteDefCollection;
            var noteGroupDefCollection = uocObject.NoteGroupDefCollection;
            var noteProfileCollection = uocObject.NoteProfileCollection;
            var noteGroupProfileCollection = uocObject.NoteGroupProfileCollection;
            var tpb = chartPropertyGroup.GetTpb();
            var notesData = NotesData.Create(noteDefCollection, noteGroupDefCollection, noteProfileCollection, noteGroupProfileCollection, analysisSetting, tpb);
            return new UocData(chartPropertyGroup, notesData);
        }

        /// <summary>
        /// 譜面プロパティ
        /// </summary>
        public ChartPropertyGroup ChartProperties => chartProperties;

        /// <summary>
        /// 譜面データ
        /// </summary>
        public NotesData NotesData => chartData;
    }
}


