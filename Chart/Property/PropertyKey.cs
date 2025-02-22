using System;
using System.Collections.Generic;

namespace UOC.Chart
{
    /// <summary>
    /// プロパティキー
    /// </summary>
    public class PropertyKey : IEquatable<PropertyKey>
    {
        private readonly string value;

        public PropertyKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            if (value.Contains(' ')) throw new ArgumentException($"キーに空白を含めることはできません。(キー: {value})");
            this.value = value;
        }

        public string Value => value;

        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyKey);
        }

        public bool Equals(PropertyKey other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(PropertyKey left, PropertyKey right)
        {
            return EqualityComparer<PropertyKey>.Default.Equals(left, right);
        }

        public static bool operator !=(PropertyKey left, PropertyKey right)
        {
            return !(left == right);
        }
    }
}
