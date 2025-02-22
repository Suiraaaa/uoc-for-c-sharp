using System;

namespace UOC.Chart
{
    public class Distance
    {
        private readonly float beatsCount;

        public Distance(float beatsCount)
        {
            this.beatsCount = beatsCount;
        }

        public static Distance CreateFromDifference(Position start, Position end, MeasureLengthProvider measureLengthProvider)
        {
            if (start == null) throw new ArgumentNullException(nameof(start));
            if (end == null) throw new ArgumentNullException(nameof(end));
            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            return new Distance(end.GetTotalBeatsCount(measureLengthProvider) - start.GetTotalBeatsCount(measureLengthProvider));
        }

        /// <summary>
        /// 四分音符単位の距離
        /// </summary>
        public float BeatsCount => beatsCount;

        /// <summary>
        /// 距離が負の方向を表すかどうか
        /// </summary>
        public bool IsNegative => beatsCount < 0;
    }
}
