using System;
using Uoc.Analyze.Speed;

namespace Uoc.Chart.Event
{
    /// <summary>
    /// スピード倍率変動イベント
    /// </summary>
    public class SpeedMultiplierChangeEvent
    {
        private readonly Position position;
        private readonly Layer layer;
        private readonly SpeedMultiplier speedMultiplier;
        private readonly Tick tick;

        public SpeedMultiplierChangeEvent(Position position, Layer layer, SpeedMultiplier speedMultiplier, MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            this.position = position ?? throw new ArgumentNullException(nameof(position));
            this.layer = layer ?? throw new ArgumentNullException(nameof(layer));
            this.speedMultiplier = speedMultiplier ?? throw new ArgumentNullException(nameof(speedMultiplier));

            if (measureLengthProvider == null) throw new ArgumentNullException(nameof(measureLengthProvider));
            if (tpb == null) throw new ArgumentNullException(nameof(tpb));
            tick = GetTick(measureLengthProvider, tpb);

        }

        public MeasureIndex MeasureIndex => position.MeasureIndex;

        public Layer Layer => layer;

        public SpeedMultiplier SpeedMultiplier => speedMultiplier;

        public Tick Tick => tick;

        private Tick GetTick(MeasureLengthProvider measureLengthProvider, Tpb tpb)
        {
            var measureLength = measureLengthProvider.GetMeasureLengthAt(position.MeasureIndex.Value);
            return position.CalculateTick(measureLength, tpb);
        }
    }
}
