using System;
using UOC.Analyze;
using UOC.Chart;

namespace UOC
{
    /// <summary>
    /// UOCデータ
    /// </summary>
    public readonly struct UOCData
    {
        private readonly ChartPropertyGroup chartProperties;
        private readonly ChartData chartData;

        public UOCData(ChartPropertyGroup chartProperties, ChartData chartData)
        {
            this.chartProperties = chartProperties ?? throw new ArgumentNullException(nameof(chartProperties));
            this.chartData = chartData ?? throw new ArgumentNullException(nameof(chartData));
        }

        public static UOCData Create(UOCString uocString, AnalysisSetting analysisSetting)
        {
            var uocObject = UOCObject.Parse(uocString);
            var chartPropertyGroup = uocObject.ChartPropertyGroup;
            var tpb = chartPropertyGroup.GetTPB();
            var chartData = ChartData.Create(uocObject.NoteProfileCollection, uocObject.NoteGroupProfileCollection, analysisSetting, tpb);
            return new UOCData(chartPropertyGroup, chartData);
        }

        /// <summary>
        /// 譜面プロパティ
        /// </summary>
        public ChartPropertyGroup ChartProperties => chartProperties;

        /// <summary>
        /// 譜面データ
        /// </summary>
        public ChartData ChartData => chartData;
    }
}


