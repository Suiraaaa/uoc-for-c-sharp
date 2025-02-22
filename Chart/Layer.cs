using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UOC.Analyze;

namespace UOC.Chart
{
    /// <summary>
    /// レイヤー
    /// </summary>
    public class Layer : IEquatable<Layer>
    {
        private const string LAYER_LINE_HEADER = "LAYER";
        private const int LAYER_MIN = 0;
        private const int LAYER_MAX = 30; // 仕様で定義されるレイヤーは30まで

        private readonly int value;

        public Layer(int value)
        {
            if (value < LAYER_MIN || LAYER_MAX < value)
                throw new ArgumentOutOfRangeException($"レイヤーは{LAYER_MIN}以上{LAYER_MAX}以内の範囲である必要があります。(入力値: {value})");

            this.value = value;
        }

        public int Value => value;

        internal static Layer FormUOCLine(UOCLine uocLine)
        {
            if (!CanFormUOCLine(uocLine)) throw new InvalidOperationException($"行「{uocLine}」はレイヤー定義行ではありません。");

            try
            {
                /*
                 * 想定入力形式
                 * "LAYER:n"（nはレイヤー値）
                 */
                var match = Regex.Match(uocLine.LineText, @"\s*LAYER\s*:\s*(\d+)\s*");
                int layer = int.Parse(match.Groups[1].Value);
                return new Layer(layer);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"行「{uocLine}」をレイヤーに変換できません。", e);
            }
        }

        internal static bool CanFormUOCLine(UOCLine uocLine)
        {
            return uocLine.LineText.Replace(" ", "")[..5] == LAYER_LINE_HEADER;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Layer);
        }

        public bool Equals(Layer other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Layer left, Layer right)
        {
            return EqualityComparer<Layer>.Default.Equals(left, right);
        }

        public static bool operator !=(Layer left, Layer right)
        {
            return !(left == right);
        }
    }
}
