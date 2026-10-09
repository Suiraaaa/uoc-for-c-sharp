using System;
using System.Collections.Generic;
using System.Numerics;
using Uoc.Chart.Event;

namespace Uoc.Chart
{
    /// <summary>
    /// <see cref="Position"/>同士の距離を表すクラス
    /// </summary>
    public class Distance : IEquatable<Distance>
    {
        private readonly float quarterNoteCount;
        private readonly Fraction? quarterNoteFraction;

        public Distance(float quarterNoteCount)
        {
            this.quarterNoteCount = quarterNoteCount;
            if (!float.IsNaN(quarterNoteCount) && !float.IsInfinity(quarterNoteCount))
            {
                quarterNoteFraction = Fraction.FromSingle(quarterNoteCount);
            }
        }

        /// <summary>
        /// 四分音符単位の距離を、分子と正の分母から正確に作成します。
        /// </summary>
        public Distance(BigInteger quarterNoteNumerator, BigInteger quarterNoteDenominator)
        {
            if (quarterNoteDenominator.Sign <= 0) throw new ArgumentOutOfRangeException(nameof(quarterNoteDenominator));
            var fraction = new Fraction(quarterNoteNumerator, quarterNoteDenominator);
            quarterNoteFraction = fraction;
            // 分子と分母を同量だけ縮小し、doubleへの変換時に両方がInfinityになることを防ぐ。
            var shift = Math.Max(0, fraction.Denominator.ToByteArray().Length * 8 - 512);
            quarterNoteCount = (float)((double)(fraction.Numerator >> shift) / (double)(fraction.Denominator >> shift));
        }

        private Distance(float quarterNoteCount, Fraction quarterNoteFraction)
        {
            this.quarterNoteCount = quarterNoteCount;
            this.quarterNoteFraction = quarterNoteFraction;
        }

        internal Fraction? QuarterNoteFraction => quarterNoteFraction;

        /// <summary>
        /// 二点間から距離を作成します。
        /// </summary>
        /// <param name="start">始点</param>
        /// <param name="end">終点</param>
        /// <param name="measureLengthProvider">小節長プロバイダ</param>
        /// <returns>二点間の距離</returns>
        public static Distance CreateFromDifference(Position start, Position end, MeasureLengthProvider measureLengthProvider)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            var quarterNoteCount = end.GetTotalQuarterNoteCount(measureLengthProvider) - start.GetTotalQuarterNoteCount(measureLengthProvider);
            var quarterNoteFraction = end.GetTotalQuarterNoteFraction(measureLengthProvider) - start.GetTotalQuarterNoteFraction(measureLengthProvider);
            return new Distance(quarterNoteCount, quarterNoteFraction);
        }

        /// <summary>
        /// 四分音符単位の距離
        /// </summary>
        public float QuarterNoteCount => quarterNoteCount;

        /// <summary>
        /// 四分音符単位の距離の正確な分子。非有限値の距離では取得できません。
        /// </summary>
        public BigInteger QuarterNoteNumerator => (quarterNoteFraction ?? throw new InvalidOperationException("非有限値の距離には整数比がありません。")).Numerator;

        /// <summary>
        /// 四分音符単位の距離の正確な分母。非有限値の距離では取得できません。
        /// </summary>
        public BigInteger QuarterNoteDenominator => (quarterNoteFraction ?? throw new InvalidOperationException("非有限値の距離には整数比がありません。")).Denominator;

        /// <summary>
        /// 距離の絶対値を求め、新たなDistanceオブジェクトとして返します。
        /// </summary>
        /// <returns>距離の絶対値</returns>
        public Distance Absolute()
        {
            if (quarterNoteFraction is Fraction fraction)
            {
                return new Distance(Math.Abs(quarterNoteCount), fraction.Numerator.Sign < 0 ? -fraction : fraction);
            }
            return new Distance(Math.Abs(quarterNoteCount));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Distance);
        }

        public bool Equals(Distance? other)
        {
            return other is not null &&
                   quarterNoteCount == other.quarterNoteCount;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(quarterNoteCount);
        }

        public static bool operator ==(Distance? left, Distance? right)
        {
            return EqualityComparer<Distance?>.Default.Equals(left, right);
        }

        public static bool operator !=(Distance? left, Distance? right)
        {
            return !(left == right);
        }
    }
}
