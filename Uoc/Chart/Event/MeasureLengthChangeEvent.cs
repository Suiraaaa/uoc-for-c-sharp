using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 小節長変動イベント
    /// </summary>
    public class MeasureLengthChangeEvent
    {
        private readonly Position position;
        private readonly MeasureLength measureLength;

        public MeasureLengthChangeEvent(Position position, MeasureLength measureLength)
        {
            if (position == null) throw new ArgumentNullException(nameof(position));
            if (!position.IsStartPoitionInMeasure()) throw new ArgumentException($"小節長変動位置は小節内の始点である必要があります。");
            if (measureLength == null) throw new ArgumentNullException(nameof(measureLength));

            this.position = position;
            this.measureLength = measureLength;
        }

        public MeasureIndex MeasureIndex => position.MeasureIndex;

        public MeasureLength MeasureLength => measureLength;
    }
}
