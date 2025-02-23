using System;
using System.Collections.Generic;
using Uoc.Chart;

namespace Uoc.Analyze
{
    internal class ChartSectionLineCollection
    {
        private readonly IReadOnlyList<ChartSectionLine> chartSectionLines;

        private ChartSectionLineCollection(IReadOnlyList<ChartSectionLine> chartSectionLines)
        {
            this.chartSectionLines = chartSectionLines ?? throw new ArgumentNullException(nameof(chartSectionLines));
        }

        public static ChartSectionLineCollection ParseUocLines(IReadOnlyList<UocLine> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            var chartSectionLineList = new List<ChartSectionLine>();
            var currentLayer = (Layer?)null;
            foreach (var line in lines)
            {
                if (Layer.CanFormUocLine(line))
                {
                    currentLayer = Layer.FormUocLine(line);
                    continue;
                }
                if (currentLayer == null) throw new Exception($"レイヤーが指定されていない行が存在します。(行: {line})");

                var chartSectionLine = ChartSectionLine.ParseUocLine(line, currentLayer);
                chartSectionLineList.Add(chartSectionLine);
            }
            return new ChartSectionLineCollection(chartSectionLineList);
        }

        public IReadOnlyList<ChartSectionLine> ChartSectionLines => chartSectionLines;
    }
}
