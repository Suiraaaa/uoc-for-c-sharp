using System;
using Uoc.Chart;

namespace Uoc.Analyze
{
    /// <summary>
    /// UOCアセットの解析設定
    /// </summary>
    public class AnalysisSetting
    {
        private readonly BasicSpeed basicSpeed;
        private readonly long minTiming;
        private readonly bool ignoreSpeedChangesAfterJudgeLine;
        private readonly int notesInstantiateTimingInterval;

        public AnalysisSetting(BasicSpeed basicSpeed, long minTiming, bool ignoreSpeedChangesAfterJudgeLine, int notesInstantiateTimingInterval)
        {
            if (notesInstantiateTimingInterval < 1) throw new ArgumentOutOfRangeException(nameof(notesInstantiateTimingInterval));

            this.basicSpeed = basicSpeed ?? throw new ArgumentNullException(nameof(basicSpeed));
            this.minTiming = minTiming;
            this.ignoreSpeedChangesAfterJudgeLine = ignoreSpeedChangesAfterJudgeLine;
            this.notesInstantiateTimingInterval = notesInstantiateTimingInterval;
        }

        /// <summary>
        /// ノートの基本移動速度
        /// </summary>
        public BasicSpeed BasicSpeed => basicSpeed;

        /// <summary>
        /// 譜面の最小タイミング
        /// </summary>
        public long MinTiming => minTiming;

        /// <summary>
        /// 判定ライン以降のスピード変動を無視するかどうか
        /// trueの場合、有効タイミング以降はスピード倍率が1.0に固定される
        /// </summary>
        public bool IgnoreSpeedChangesAfterJudgeLine => ignoreSpeedChangesAfterJudgeLine;

        /// <summary>
        /// ノート生成タイミングの間隔(ミリ秒)
        /// この値が大きいほど解析が高速になりますが、同時に存在するノート数が増加します。
        /// 推奨値：500 ~ 2000
        /// -----値による動作の比較-----
        /// 値:1000  → ノートの生成タイミングは1秒間隔になります。そのため、ノート生成タイミングはノート生成位置より手前を表すことがあります。
        /// 値:1     → ノート生成タイミングはノート生成位置ちょうどを表すようになりますが、解析に時間がかかるようになります。
        /// </summary>
        public int NotesInstantiateTimingInterval => notesInstantiateTimingInterval;
    }
}
