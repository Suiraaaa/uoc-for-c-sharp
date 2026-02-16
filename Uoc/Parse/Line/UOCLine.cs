using System;
using System.Text.RegularExpressions;

namespace Uoc.Parse.Line
{
    /// <summary>
    /// UOCの行情報
    /// </summary>
    internal class UocLine
    {
        private static readonly Regex sectionStartLineRegex = new(@"^\s*(@)\s*(.*)$", RegexOptions.Compiled);
        private static readonly Regex dataLineRegex = new(@"^\s*(#)\s*(.*)$", RegexOptions.Compiled);

        private readonly LineType lineType;
        private readonly string lineText;
        private readonly string originalLineText;

        private UocLine(LineType lineType, string lineText, string originalLineText)
        {
            this.lineType = lineType;
            this.lineText = lineText;
            this.originalLineText = originalLineText;
        }

        public static UocLine Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new UocLine(LineType.Empty, string.Empty, string.Empty);
            }

            // セクション開始行の場合のパース
            if (line.Contains('@'))
            {
                try
                {
                    var match = sectionStartLineRegex.Match(line);
                    if (!match.Success)
                    {
                        throw new FormatException("正規表現によるパースに失敗しました。");
                    }
                    return new UocLine(LineType.SectionHead, match.Groups[2].Value, line);
                }
                catch (Exception e)
                {
                    throw new Exception($"文字列\"{line}\"をUOC行にパースできませんでした。", e);
                }
            }

            // データ行の場合のパース
            if (line.Contains('#'))
            {
                try
                {
                    var match = dataLineRegex.Match(line);
                    if (!match.Success)
                    {
                        throw new FormatException("正規表現によるパースに失敗しました。");
                    }
                    return new UocLine(LineType.Data, match.Groups[2].Value, line);
                }
                catch (Exception e)
                {
                    throw new Exception($"文字列\"{line}\"をUOC行にパースできませんでした。", e);
                }
            }

            // 上記に該当しない場合はコメント行とする
            return new UocLine(LineType.Comment, line, line);
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
