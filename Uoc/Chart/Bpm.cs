using System;
using System.Collections.Generic;

namespace Uoc.Chart
{
    /// <summary>
    /// BPM
    /// </summary>
    public class Bpm : IEquatable<Bpm>
    {
        private readonly float value;

        public Bpm(float value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException($"BPMは0より大きい値である必要があります。(入力値: {value})");
            this.value = value;
        }

        public float Value => value;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Bpm);
        }

        public bool Equals(Bpm? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(Bpm? left, Bpm? right)
        {
            return EqualityComparer<Bpm?>.Default.Equals(left, right);
        }

        public static bool operator !=(Bpm? left, Bpm? right)
        {
            return !(left == right);
        }
    }
}
