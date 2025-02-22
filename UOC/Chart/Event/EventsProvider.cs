using System;

namespace UOC.Chart
{
    /// <summary>
    /// 各種イベントを提供するクラス
    /// </summary>
    public class EventsProvider
    {
        private readonly BPMProvider bpmProvider;
        private readonly SpeedMultiplierProvider speedMultiplierProvider;
        private readonly MeasureLengthProvider measureLengthProvider;

        public EventsProvider(BPMProvider bpmProvider, SpeedMultiplierProvider speedMultiplierProvider, MeasureLengthProvider measureLengthProvider)
        {
            this.bpmProvider = bpmProvider ?? throw new ArgumentNullException(nameof(bpmProvider));
            this.speedMultiplierProvider = speedMultiplierProvider ?? throw new ArgumentNullException(nameof(speedMultiplierProvider));
            this.measureLengthProvider = measureLengthProvider ?? throw new ArgumentNullException(nameof(measureLengthProvider));
        }

        public BPMProvider BPMProvider => bpmProvider;

        public SpeedMultiplierProvider SpeedMultiplierProvider => speedMultiplierProvider;

        public MeasureLengthProvider MeasureLengthProvider => measureLengthProvider;
    }
}
