using System;
using System.Collections.Generic;
using System.Linq;

namespace UOC.Chart
{
    /// <summary>
    /// BPM情報を提供するクラス
    /// </summary>
    public class BPMProvider
    {
        private readonly IReadOnlyList<BPMChangeEvent> bpmChangeEvents;

        public BPMProvider(IReadOnlyList<BPMChangeEvent> bpmChangeEvents)
        {
            if (bpmChangeEvents == null) throw new ArgumentNullException(nameof(bpmChangeEvents));
            if (bpmChangeEvents.Count == 0) throw new ArgumentException("BPM変動イベントが含まれていません。");

            // 小節
            // 小節番号・チックで昇順ソート
            var sortedBPMChangeEvents = bpmChangeEvents.OrderBy(x => x.MeasureIndex.Value).ThenBy(x => x.Tick.Value).ToList();
            if (sortedBPMChangeEvents[0].MeasureIndex.Value != 0 || sortedBPMChangeEvents[0].Tick.Value != 0) throw new ArgumentException("譜面始点のBPMが含まれていません。");

            this.bpmChangeEvents = sortedBPMChangeEvents;
        }

        /// <summary>
        /// 指定された小節の始点BPMを取得します。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節の始点BPM</returns>
        public BPM GetMeasureStartBPM(int measureIndex)
        {
            for (int i = bpmChangeEvents.Count - 1; i >= 0; i--)
            {
                if (bpmChangeEvents[i].MeasureIndex.Value == measureIndex && bpmChangeEvents[i].Tick.Value == 0)
                {
                    return bpmChangeEvents[i].BPM;
                }
                if (bpmChangeEvents[i].MeasureIndex.Value < measureIndex)
                {
                    return bpmChangeEvents[i].BPM;
                }
            }
            return bpmChangeEvents[0].BPM;
        }

        /// <summary>
        /// 指定された小節内のBPM変動イベントリストを取得します。
        /// 小節始点の変動イベントは含まれません。
        /// </summary>
        /// <param name="measureIndex">小節番号</param>
        /// <returns>指定された小節内のBPM変動イベントリスト</returns>
        public IReadOnlyList<BPMChangeEvent> GetBPMChangeEventsAt(int measureIndex)
        {
            return bpmChangeEvents.Where(x => x.MeasureIndex.Value == measureIndex && x.Tick.Value != 0).ToList();
        }
    }
}
