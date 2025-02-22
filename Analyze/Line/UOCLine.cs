using System;
using System.Text.RegularExpressions;

namespace UOC.Analyze
{
    /// <summary>
    /// UOCの行情報
    /// </summary>
    internal class UOCLine
    {
        private readonly LineType lineType;
        private readonly string lineText;
        private readonly string originalLineText;

        private UOCLine(LineType lineType, string lineText, string originalLineText)
        {


            this.lineType = lineType;
            this.lineText = lineText;
            this.originalLineText = originalLineText;
        }

        public static UOCLine Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new UOCLine(LineType.Empty, string.Empty, string.Empty);
            }
            if (line.Contains('@'))
            {
                try
                {
                    var match = Regex.Match(line, @"^\s*(@)\s*(.*)$");
                    if (!match.Success)
                    {
                        throw new Exception("正規表現によるパースに失敗しました。");
                    }
                    return new UOCLine(LineType.SectionHead, match.Groups[2].Value, line);
                }
                catch (Exception e)
                {
                    throw new Exception($"文字列\"{line}\"をUOC行にパースできませんでした。", e);
                }
            }
            if (line.Contains('#'))
            {
                try
                {
                    var match = Regex.Match(line, @"^\s*(#)\s*(.*)$"); if (!match.Success)
                    {
                        throw new Exception("正規表現によるパースに失敗しました。");
                    }
                    return new UOCLine(LineType.Data, match.Groups[2].Value, line);
                }
                catch (Exception e)
                {
                    throw new Exception($"文字列\"{line}\"をUOC行にパースできませんでした。", e);
                }
            }
            return new UOCLine(LineType.Comment, line, line);
        }

        /// <summary>
        /// 行種別
        /// </summary>
        public LineType LineType => lineType;

        /// <summary>
        /// 行文字列
        /// 先頭符号は含まれない（ただし空白は含まれる）
        /// </summary>
        public string LineText => lineText;

        public override string ToString()
        {
            return $"\"{originalLineText}\"";
        }
    }
}
