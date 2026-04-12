using System;
using System.Collections.Generic;

namespace Uoc.Chart
{
    /// <summary>
    /// TPB（一拍の分解能）
    /// </summary>
    public class Tpb : IEquatable<Tpb>
    {
        private readonly int value;

        public Tpb(int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException($"TPBは0より大きい値を入力してください。(入力値: {value})");
            this.value = value;
        }

        public int Value => value;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Tpb);
        }

        public bool Equals(Tpb? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Tpb? left, Tpb? right)
        {
            return EqualityComparer<Tpb?>.Default.Equals(left, right);
        }

        public static bool operator !=(Tpb? left, Tpb? right)
        {
            return !(left == right);
        }
    }
}
