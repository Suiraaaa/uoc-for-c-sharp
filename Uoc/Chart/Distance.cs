using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 譜面位置同士の距離を表すクラス
    /// </summary>
    public class Distance
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
        /// 距離が負の方向を表すかどうか
        /// </summary>
        public bool IsNegative => quarterNoteCount < 0;

        /// <summary>
        /// 距離の絶対値を求め、新たなDistanceオブジェクトとして返します。
        /// </summary>
        /// <returns>距離の絶対値</returns>
        public Distance Absolute()
        {
            return new Distance(Math.Abs(quarterNoteCount));
        }
    }
}
