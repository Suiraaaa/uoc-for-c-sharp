using System;
using System.Collections.Generic;
using UOC.Chart;

namespace UOC.Analyze
{
    internal class ChartSectionLineCollection
    {
        private readonly IReadOnlyList<ChartSectionLine> chartSectionLines;

        private ChartSectionLineCollection(IReadOnlyList<ChartSectionLine> chartSectionLines)
        {
            this.chartSectionLines = chartSectionLines ?? throw new ArgumentNullException(nameof(chartSectionLines));
        }

        public static ChartSectionLineCollection ParseUOCLines(IReadOnlyList<UOCLine> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            var chartSectionLineList = new List<ChartSectionLine>();
            Layer currentLayer = null;
            foreach (var line in lines)
            {
                if (Layer.CanFormUOCLine(line))
                {
                    currentLayer = Layer.FormUOCLine(line);
                    continue;
                }
                if (currentLayer == null) throw new Exception($"レイヤーが指定されていない行が存在します。(行: {line})");

                var chartSectionLine = ChartSectionLine.ParseUOCLine(line, currentLayer);
                chartSectionLineList.Add(chartSectionLine);
            }
            return new ChartSectionLineCollection(chartSectionLineList);
        }

        public IReadOnlyList<ChartSectionLine> ChartSectionLines => chartSectionLines;
    }
}
