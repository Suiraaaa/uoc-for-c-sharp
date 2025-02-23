using System;
using System.Collections.Generic;

namespace Uoc.Chart
{
    /// <summary>
    /// ノートの移動速度倍率
    /// </summary>
    public class SpeedMultiplier : IEquatable<SpeedMultiplier>
    {
        private readonly float multiplier;

        public SpeedMultiplier(float multiplier)
        {
            this.multiplier = multiplier;
        }

        public static SpeedMultiplier One => new(1f);

        public float Multiplier => multiplier;

        public override bool Equals(object obj)
        {
            return Equals(obj as SpeedMultiplier);
        }

        public bool Equals(SpeedMultiplier? other)
        {
            return other is not null &&
                   multiplier == other.multiplier;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(multiplier);
        }

        public static bool operator ==(SpeedMultiplier? left, SpeedMultiplier? right)
        {
            return EqualityComparer<SpeedMultiplier?>.Default.Equals(left, right);
        }

        public static bool operator !=(SpeedMultiplier? left, SpeedMultiplier? right)
        {
            return !(left == right);
        }
    }
}
