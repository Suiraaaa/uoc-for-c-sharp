using System;
using System.Numerics;

namespace Uoc.Chart
{
    // 位置と四分音符数の整数比を保持し、途中計算のオーバーフローと丸めを防ぐ。
    internal readonly struct Fraction
    {
        internal Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero) throw new DivideByZeroException();
            if (denominator.Sign < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }
            var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
            Numerator = numerator / gcd;
            Denominator = denominator / gcd;
        }

        internal BigInteger Numerator { get; }
        internal BigInteger Denominator { get; }
        internal static Fraction Zero => new(0, 1);

        internal static Fraction FromSingleExact(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value));
            var bits = BitConverter.SingleToInt32Bits(value);
            var exponent = (bits >> 23) & 0xff;
            var significand = bits & 0x7fffff;
            if (exponent != 0) significand |= 1 << 23;
            var shift = exponent == 0 ? -149 : exponent - 150;
            var numerator = new BigInteger(significand);
            if (bits < 0) numerator = -numerator;
            return shift >= 0
                ? new Fraction(numerator << shift, 1)
                : new Fraction(numerator, BigInteger.One << -shift);
        }

        internal static Fraction FromSingle(float value)
        {
            var exact = FromSingleExact(value);
            if (exact.Numerator.IsZero || exact.Denominator.IsOne) return exact;

            var positive = Math.Abs(value);
            var bits = BitConverter.SingleToInt32Bits(positive);
            var center = FromSingleExact(positive);
            var lower = (center + FromSingleExact(BitConverter.Int32BitsToSingle(bits - 1))) / new Fraction(2, 1);
            var upper = (center + FromSingleExact(BitConverter.Int32BitsToSingle(bits + 1))) / new Fraction(2, 1);
            var includeBoundary = (bits & 1) == 0;

            // 同じfloatへ丸められる区間内で連分数の収束分数を探す。
            // 固定epsilonを使わず、入力で区別できる位置を保持する。
            var numerator = center.Numerator;
            var denominator = center.Denominator;
            var previousNumerator = BigInteger.Zero;
            var currentNumerator = BigInteger.One;
            var previousDenominator = BigInteger.One;
            var currentDenominator = BigInteger.Zero;
            while (!denominator.IsZero)
            {
                var quotient = BigInteger.DivRem(numerator, denominator, out var remainder);
                var nextNumerator = quotient * currentNumerator + previousNumerator;
                var nextDenominator = quotient * currentDenominator + previousDenominator;
                var candidate = new Fraction(nextNumerator, nextDenominator);
                var lowerComparison = candidate.CompareTo(lower);
                var upperComparison = candidate.CompareTo(upper);
                if ((lowerComparison > 0 && upperComparison < 0)
                    || (includeBoundary && lowerComparison >= 0 && upperComparison <= 0))
                {
                    return value < 0 ? -candidate : candidate;
                }
                previousNumerator = currentNumerator;
                currentNumerator = nextNumerator;
                previousDenominator = currentDenominator;
                currentDenominator = nextDenominator;
                numerator = denominator;
                denominator = remainder;
            }
            return exact;
        }

        internal int CompareTo(Fraction other)
        {
            return (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);
        }

        public static Fraction operator +(Fraction left, Fraction right)
        {
            return new Fraction(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);
        }

        public static Fraction operator -(Fraction left, Fraction right) => left + -right;
        public static Fraction operator -(Fraction value) => new(-value.Numerator, value.Denominator);
        public static Fraction operator *(Fraction left, Fraction right) => new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);
        public static Fraction operator /(Fraction left, Fraction right) => new(left.Numerator * right.Denominator, left.Denominator * right.Numerator);
    }
}
