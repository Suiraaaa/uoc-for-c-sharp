using System;
using System.Collections.Generic;
using System.IO;

namespace UOC.Analyze
{
    /// <summary>
    /// UOCの行情報コレクション
    /// </summary>
    internal class UOCLineCollection
    {
        private readonly IReadOnlyList<UOCLine> lines;

        private UOCLineCollection(IReadOnlyList<UOCLine> lines)
        {
            this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
        }

        public static UOCLineCollection Parse(UOCString uocString)
        {
            var lines = new List<UOCLine>();
            var stringReader = new StringReader(uocString.String);
            while (stringReader.Peek() > -1)
            {
                var line = UOCLine.Parse(stringReader.ReadLine());
                lines.Add(line);
            }
            return new UOCLineCollection(lines);
        }

        /// <summary>
        /// 指定されたセクションに所属する行リストを取得します。
        /// </summary>
        /// <param name="sectionType">対象セクション</param>
        /// <returns>指定されたセクションに所属する行リスト</returns>
        public IReadOnlyList<UOCLine> GetLinesIn(SectionType sectionType)
        {
            var targetLines = new List<UOCLine>();
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
