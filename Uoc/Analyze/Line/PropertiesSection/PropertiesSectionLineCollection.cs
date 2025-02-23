using System.Collections.Generic;
using System.Linq;
using Uoc.Chart;

namespace Uoc.Analyze
{
    /// <summary>
    /// PROPERTIESセクションに所属する行のコレクション
    /// </summary>
    internal class PropertiesSectionLineCollection
    {
        private readonly IReadOnlyList<PropertiesSectionLine> propertiesSectionLines;

        private PropertiesSectionLineCollection(IReadOnlyList<PropertiesSectionLine> propertiesSectionLines)
        {
            this.propertiesSectionLines = propertiesSectionLines;
        }

        public static PropertiesSectionLineCollection ParseUocLines(IReadOnlyList<UocLine> lines)
        {
            var propertiesSectionLines = lines.Select(x => PropertiesSectionLine.ParseUocLine(x)).ToList();
            return new PropertiesSectionLineCollection(propertiesSectionLines);
        }

        /// <summary>
        /// 行情報から譜面プロパティグループを作成します。
        /// </summary>
        /// <returns>譜面プロパティグループ</returns>
        public ChartPropertyGroup CreateChartPropertyGroup()
        {
            var properties = propertiesSectionLines.Select(x => x.ToProperty()).ToList();
            var propertyGroup = new PropertyGroup(properties);
            return new ChartPropertyGroup(propertyGroup);
        }
    }
}
