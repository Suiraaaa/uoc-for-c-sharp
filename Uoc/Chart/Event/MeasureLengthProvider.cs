using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart
{
    /// <summary>
    /// 小節長情報を提供するクラス
    /// </summary>
    public class MeasureLengthProvider
    {
        private readonly IReadOnlyList<MeasureLengthChangeEvent> measureLengthChangeEvents;

        public MeasureLengthProvider(IReadOnlyList<MeasureLengthChangeEvent> measureLengthChangeEvents)
        {
            if (measureLengthChangeEvents == null) throw new ArgumentNullException(nameof(measureLengthChangeEvents));
            if (measureLengthChangeEvents.Count == 0) throw new ArgumentException("小節長変動イベントが含まれていません。");

            var sortedMeasureLengthChangeEvents = measureLengthChangeEvents.OrderBy(x => x.MeasureIndex.Value).ToList();
            if (sortedMeasureLengthChangeEvents[0].MeasureIndex.Value != 0) throw new ArgumentException("譜面始点の小節長が含まれていません。");

            this.measureLengthChangeEvents = sortedMeasureLengthChangeEvents; ;
        }

        /// <summary>
        /// 指定された小節の小節長を取得します。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節の小節長</returns>
        public MeasureLength GetMeasureLengthAt(int measureIndex)
        {
            for (int i = measureLengthChangeEvents.Count - 1; i >= 0; i--)
            {
                if (measureLengthChangeEvents[i].MeasureIndex.Value <= measureIndex)
                {
                    return measureLengthChangeEvents[i].MeasureLength;
                }
            }
            return measureLengthChangeEvents[0].MeasureLength;
        }
    }
}
