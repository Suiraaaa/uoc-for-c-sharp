using System;
using System.Collections.Generic;

namespace Uoc.Chart
{
    /// <summary>
    /// ティック
    /// </summary>
    public class Tick : IEquatable<Tick>
    {
        private readonly int value;

        public Tick(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException($"Tickは0以上の値である必要があります。(入力値: {value})");
            this.value = value;
        }

        public Tick(float value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException($"Tickは0以上の値である必要があります。(入力値: {value})");
            this.value = (int)Math.Floor(value); // 小数点以下切り捨て
        }

        public int Value => value;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Tick);
        }

        public bool Equals(Tick? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Tick? left, Tick? right)
        {
            return EqualityComparer<Tick?>.Default.Equals(left, right);
        }

        public static bool operator !=(Tick? left, Tick? right)
        {
            return !(left == right);
        }
    }
}
