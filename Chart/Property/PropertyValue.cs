using System;

namespace UOC.Chart
{
    /// <summary>
    /// プロパティ値
    /// </summary>
    public class PropertyValue
    {
        private readonly string value;
        private readonly bool isEmpty;

        public PropertyValue()
        {
            value = string.Empty;
            isEmpty = true;
        }

        public PropertyValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            this.value = value;
            isEmpty = false;
        }

        public static PropertyValue Empty => new();

        /// <summary>
        /// プロパティ値を String として取得します。
        /// </summary>
        /// <returns>プロパティ値（String）</returns>
        public string AsString()
        {
            if (!HasValue()) throw new InvalidOperationException("プロパティは値を持っていません。");
            return value;
        }

        /// <summary>
        /// プロパティ値を Int として取得します。
        /// </summary>
        /// <returns>プロパティ値（Int）</returns>
        public int AsInt()
        {
            if (!HasValue()) throw new InvalidOperationException("プロパティは値を持っていません。");
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            throw new InvalidCastException($"プロパティ値 \"{value}\"をInt型にパースできませんでした。");
        }

        /// <summary>
        /// プロパティ値を Float として取得します。
        /// </summary>
        /// <returns>プロパティ値（Float）</returns>
        public float AsFloat()
        {
            if (!HasValue()) throw new InvalidOperationException("プロパティは値を持っていません。");
            if (float.TryParse(value, out float result))
            {
                return result;
            }
            throw new InvalidCastException($"プロパティ値 \"{value}\"をFloat型にパースできませんでした。");
        }

        /// <summary>
        /// プロパティ値を Boolean として取得します。
        /// </summary>
        /// <returns>プロパティ値（Boolean）</returns>
        public bool AsBoolean()
        {
            if (!HasValue()) throw new InvalidOperationException("プロパティは値を持っていません。");
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            throw new InvalidCastException($"プロパティ値 \"{value}\"をBoolean型にパースできませんでした。");
        }

        /// <summary>
        /// 値を持っているかどうかを取得します。
        /// </summary>
        /// <returns>値を持っているかどうか</returns>
        public bool HasValue()
        {
            return !isEmpty;
        }
    }
}
