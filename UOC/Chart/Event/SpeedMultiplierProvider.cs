using System;
using System.Collections.Generic;
using System.Linq;

namespace UOC.Chart
{
    /// <summary>
    /// スピード倍率情報を提供するクラス
    /// </summary>
    public class SpeedMultiplierProvider
    {
        private readonly IReadOnlyList<SpeedChangeEvent> speedChangeEvents;

        public SpeedMultiplierProvider(IReadOnlyList<SpeedChangeEvent> speedChangeEvents)
        {
            if (speedChangeEvents == null) throw new ArgumentNullException(nameof(speedChangeEvents));

            // 小節番号・チックで昇順ソート
            var sortedSpeedChangeEvents = speedChangeEvents.OrderBy(x => x.MeasureIndex.Value).ThenBy(x => x.Tick.Value).ToList();
            this.speedChangeEvents = sortedSpeedChangeEvents;
        }

        /// <summary>
        /// 指定された小節の始点スピード倍率を取得します。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <param name="layer">対象レイヤー</param>
        /// <returns>指定された小節の始点スピード倍率</returns>
        public SpeedMultiplier GetSpeedMultiplierAt(int measureIndex, Layer layer)
        {
            if (measureIndex < 0) throw new ArgumentOutOfRangeException(nameof(measureIndex));
            for (int i = speedChangeEvents.Count - 1; i >= 0; i--)
            {
                if (speedChangeEvents[i].Layer != layer) continue;
                if (speedChangeEvents[i].MeasureIndex.Value == measureIndex && speedChangeEvents[i].Tick.Value == 0)
                {
                    return speedChangeEvents[i].SpeedMultiplier;
                }
                if (speedChangeEvents[i].MeasureIndex.Value < measureIndex)
                {
                    return speedChangeEvents[i].SpeedMultiplier;
                }
            }
            return SpeedMultiplier.One;
        }

        /// <summary>
        /// 指定された小節範囲内のスピード変動イベントリストを取得します。
        /// ただし、開始範囲の始点スピード変動は含まれません。
        /// </summary>
        /// <param name="startMeasureIndex">開始小節番号</param>
        /// <param name="endMeasureIndex">終了小節番号</param>
        /// <param name="layer">対象レイヤー</param>
        /// <returns>指定された小節範囲内のスピード変動イベントリスト</returns>
        public IReadOnlyList<SpeedChangeEvent> GetSpeedChangeEventsWithoutStartPoint(int startMeasureIndex, int endMeasureIndex, Layer layer)
        {
            if (startMeasureIndex < 0) throw new ArgumentOutOfRangeException(nameof(startMeasureIndex));
            if (endMeasureIndex < 0) throw new ArgumentOutOfRangeException(nameof(endMeasureIndex));
            if (startMeasureIndex > endMeasureIndex) throw new ArgumentException();

            return speedChangeEvents.Where(x => x.Layer == layer &&
                                                !(x.MeasureIndex.Value == startMeasureIndex && x.Tick.Value == 0) && // 範囲始点イベントを除く
                                                x.MeasureIndex.Value >= startMeasureIndex &&
                                                x.MeasureIndex.Value <= endMeasureIndex).ToList();
        }
    }
}
