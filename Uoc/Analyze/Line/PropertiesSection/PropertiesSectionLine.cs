using System;
using System.Text.RegularExpressions;
using Uoc.Chart;

namespace Uoc.Analyze
{
    /// <summary>
    /// PROPERTIESセクションに所属する行
    /// </summary>
    internal class PropertiesSectionLine
    {
        private readonly PropertyKey key;
        private readonly PropertyValue value;

        public PropertiesSectionLine(PropertyKey key, PropertyValue value)
        {
            this.key = key ?? throw new ArgumentNullException(nameof(key));
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static PropertiesSectionLine ParseUocLine(UocLine line)
        {
            try
            {
                /*
                 * 想定入力形式
                 * "nnn:"mmm""（nnnはプロパティキー, mmmはプロパティ値）
                 */
                var match = Regex.Match(line.LineText, @"^\s*(\S+)\s*:\s*\""(.*)\""\s*$");
                if (!match.Success)
                {
                    throw new Exception("正規表現によるパースに失敗しました。");
                }
                var key = new PropertyKey(match.Groups[1].Value);
                var value = new PropertyValue(match.Groups[2].Value);
                return new PropertiesSectionLine(key, value);
            }
            catch (Exception e)
            {
                throw new Exception($"行{line}をPROPERTIESセクション行にパースできませんでした。", e);
            }
        }

        public Property ToProperty()
        {
            return new Property(key, value);
        }
    }
}
