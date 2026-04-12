using System;

namespace Uoc.Chart.Event
{
    /// <summary>
    /// 各種イベントを提供するクラス
    /// </summary>
    public class EventProviders
    {
        private readonly BpmProvider bpmProvider;
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly MeasureLengthProvider measureLengthProvider;

        internal EventProviders(BpmProvider bpmProvider, SpeedMultiplierProvider speedMultiplierProvider, MeasureLengthProvider measureLengthProvider)
        {
            this.bpmProvider = bpmProvider ?? throw new ArgumentNullException(nameof(bpmProvider));
            this.speedMultiplierProvider = speedMultiplierProvider ?? throw new ArgumentNullException(nameof(speedMultiplierProvider));
            this.measureLengthProvider = measureLengthProvider ?? throw new ArgumentNullException(nameof(measureLengthProvider));
        }

        public BpmProvider BpmProvider => bpmProvider;

        public SpeedMultiplierProvider SpeedMultiplierProvider => speedMultiplierProvider;

        public MeasureLengthProvider MeasureLengthProvider => measureLengthProvider;
    }
}
