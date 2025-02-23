using System;
using Uoc.Chart.Notes;

namespace Uoc.Chart
{
    /// <summary>
    /// BPM変動イベント
    /// </summary>
    public class BpmChangeEvent
    {
        private readonly Position position;
        private readonly Bpm bpm;
        private readonly Tick tick;

        public BpmChangeEvent(Position position, Bpm bpm, MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            this.position = position ?? throw new ArgumentNullException(nameof(position));
            this.bpm = bpm ?? throw new ArgumentNullException(nameof(bpm));

            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            if (tpb == null) throw new ArgumentNullException(nameof(tpb));
            tick = GetTick(measureLengthProvider, tpb);
        }

        public MeasureIndex MeasureIndex => position.MeasureIndex;

        public Bpm Bpm => bpm;

        public Tick Tick => tick;

        private Tick GetTick(MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(position.MeasureIndex.Value);
            return position.CalculateTick(measureLength, tpb);
        }
    }
}
