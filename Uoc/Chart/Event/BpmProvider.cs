using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart.Event
{
    /// <summary>
    /// BPM情報を提供するクラス
    /// </summary>
    public class BpmProvider
    {
        private readonly IReadOnlyList<BpmChangeEvent> bpmChangeEvents;

        internal BpmProvider(IReadOnlyList<BpmChangeEvent> bpmChangeEvents)
        {
            if (bpmChangeEvents == null) throw new ArgumentNullException(nameof(bpmChangeEvents));
            if (bpmChangeEvents.Count == 0) throw new ArgumentException("BPM変動イベントが含まれていません。");

            // 小節
            // 小節番号・ティックで昇順ソート
            var sortedBpmChangeEvents = bpmChangeEvents.OrderBy(x => x.MeasureIndex.Value).ThenBy(x => x.Tick.Value).ToList();
            if (sortedBpmChangeEvents[0].MeasureIndex.Value != 0 || sortedBpmChangeEvents[0].Tick.Value != 0) throw new ArgumentException("譜面始点のBpmが含まれていません。");

            this.bpmChangeEvents = sortedBpmChangeEvents;
        }

        /// <summary>
        /// 指定された小節の始点BPMを取得します。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節の始点BPM</returns>
        public Bpm GetMeasureStartBpm(int measureIndex)
        {
            for (int i = bpmChangeEvents.Count - 1; i >= 0; i--)
            {
                if (bpmChangeEvents[i].MeasureIndex.Value == measureIndex && bpmChangeEvents[i].Tick.Value == 0)
                {
                    return bpmChangeEvents[i].Bpm;
                }
                if (bpmChangeEvents[i].MeasureIndex.Value < measureIndex)
                {
                    return bpmChangeEvents[i].Bpm;
                }
            }
            return bpmChangeEvents[0].Bpm;
        }

        /// <summary>
        /// 指定された小節内のBPM変動イベントリストを取得します。
        /// 小節始点の変動イベントは含まれません。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節内のBPM変動イベントリスト</returns>
        public IReadOnlyList<BpmChangeEvent> GetBpmChangeEventsAt(int measureIndex)
        {
            return bpmChangeEvents.Where(x => x.MeasureIndex.Value == measureIndex && x.Tick.Value != 0).ToList();
        }
    }
}
