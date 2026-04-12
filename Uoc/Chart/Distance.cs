using System;
using System.Collections.Generic;
using Uoc.Chart.Event;

namespace Uoc.Chart
{
    /// <summary>
    /// <see cref="Position"/>同士の距離を表すクラス
    /// </summary>
    public class Distance : IEquatable<Distance>
    {
        private readonly float quarterNoteCount;

        public Distance(float quarterNoteCount)
        {
            this.quarterNoteCount = quarterNoteCount;
        }

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
            return new Distance(end.GetTotalQuarterNoteCount(measureLengthProvider) - start.GetTotalQuarterNoteCount(measureLengthProvider));
        }

        /// <summary>
        /// 四分音符単位の距離
        /// </summary>
        public float QuarterNoteCount => quarterNoteCount;

        /// <summary>
        /// 距離の絶対値を求め、新たなDistanceオブジェクトとして返します。
        /// </summary>
        /// <returns>距離の絶対値</returns>
        public Distance Absolute()
        {
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
