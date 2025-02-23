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
        private readonly ChartData chartData;

        public UocData(ChartPropertyGroup chartProperties, ChartData chartData)
        {
            this.chartProperties = chartProperties ?? throw new ArgumentNullException(nameof(chartProperties));
            this.chartData = chartData ?? throw new ArgumentNullException(nameof(chartData));
        }

        public static UocData Create(UocString uocString, AnalysisSetting analysisSetting)
        {
            var uocObject = UocParser.Parse(uocString);
            var chartPropertyGroup = uocObject.ChartPropertyGroup;
            var tpb = chartPropertyGroup.GetTpb();
            var chartData = ChartData.Create(uocObject.NoteProfileCollection, uocObject.NoteGroupProfileCollection, analysisSetting, tpb);
            return new UocData(chartPropertyGroup, chartData);
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


