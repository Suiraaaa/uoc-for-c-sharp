using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 各種イベントを提供するクラス
    /// </summary>
    public class EventsProvider
    {
        private readonly BpmProvider bpmProvider;
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly MeasureLengthProvider measureLengthProvider;

        public EventsProvider(BpmProvider bpmProvider, SpeedMultiplierProvider speedMultiplierProvider, MeasureLengthProvider measureLengthProvider)
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
