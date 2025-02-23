using System;
using System.Collections.Generic;
using System.IO;

namespace Uoc.Analyze
{
    /// <summary>
    /// UOCの行情報コレクション
    /// </summary>
    internal class UocLineCollection
    {
        private readonly IReadOnlyList<UocLine> lines;

        private UocLineCollection(IReadOnlyList<UocLine> lines)
        {
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
        }

        /// <summary>
        /// UOC文字列から行情報のコレクションを生成します。
        /// </summary>
        /// <param name="uocString">パース対象のUOC文字列</param>
        /// <returns>生成されたUocLineCollection</returns>
        public static UocLineCollection Parse(UocString uocString)
        {
            var lines = new List<UocLine>();
            var stringReader = new StringReader(uocString.Value);
            while (stringReader.Peek() > -1)
            {
                var line = UocLine.Parse(stringReader.ReadLine());
                lines.Add(line);
            }
            return new UocLineCollection(lines);
        }

        /// <summary>
        /// 指定されたセクションに所属する行リストを取得します。
        /// </summary>
        /// <param name="sectionType">対象セクション</param>
        /// <returns>指定されたセクションに所属する行リスト</returns>
        public IReadOnlyList<UocLine> GetLinesIn(SectionType sectionType)
        {
            var targetLines = new List<UocLine>();
            var currentSection = SectionType.None;
            foreach (var line in lines)
            {
                if (line.LineType == LineType.Comment || line.LineType == LineType.Empty)
                {
                    continue;
                }
                if (line.LineType == LineType.SectionHead)
                {
                    currentSection = SectionTypeMapper.GetSectionType(line.LineText);
                    continue;
                }
                if (currentSection == SectionType.None)
                {
                    throw new Exception($"セクションが指定されていない行が存在します。(行: {line})");
                }
                if (currentSection != sectionType) continue;
                targetLines.Add(line);
            }
            return targetLines;
        }
    }
}
