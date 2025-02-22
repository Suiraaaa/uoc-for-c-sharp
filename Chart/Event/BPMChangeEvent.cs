using System;
using UOC.Chart.Notes;

namespace UOC.Chart
{
    /// <summary>
    /// BPM変動イベント
    /// </summary>
    public class BPMChangeEvent
    {
        private readonly Position position;
        private readonly BPM bpm;
        private readonly Tick tick;

        public BPMChangeEvent(Position position, BPM bpm, MeasureLengthProvider measureLengthProvider, TPB tpb)
        {
            this.position = position ?? throw new ArgumentNullException(nameof(position));
            this.bpm = bpm ?? throw new ArgumentNullException(nameof(bpm));

            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            if (tpb == null) throw new ArgumentNullException(nameof(tpb));
            tick = GetTick(measureLengthProvider, tpb);
        }

        public MeasureIndex MeasureIndex => position.MeasureIndex;

        public BPM BPM => bpm;

        public Tick Tick => tick;

        private Tick GetTick(MeasureLengthProvider measureLengthProvider, TPB tpb)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(position.MeasureIndex.Value);
            return position.CalculateTick(measureLength, tpb);
        }
    }
}
