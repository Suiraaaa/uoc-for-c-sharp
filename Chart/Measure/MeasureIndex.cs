using System;
using System.Collections.Generic;

namespace UOC.Chart
{
    /// <summary>
    /// 小節番号
    /// </summary>
    public class MeasureIndex : IEquatable<MeasureIndex>
    {
        private readonly int value;

        public MeasureIndex(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException($"小節番号は0以上の値である必要があります。(入力値: {value})");
            this.value = value;
        }

        public int Value => value;

        public override bool Equals(object obj)
        {
            return Equals(obj as MeasureIndex);
        }

        public bool Equals(MeasureIndex other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(MeasureIndex left, MeasureIndex right)
        {
            return EqualityComparer<MeasureIndex>.Default.Equals(left, right);
        }

        public static bool operator !=(MeasureIndex left, MeasureIndex right)
        {
            return !(left == right);
        }
    }
}
