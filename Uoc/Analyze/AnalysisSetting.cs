using System;
using Uoc.Analyze.Speed;

namespace Uoc.Analyze
{
    /// <summary>
    /// UOCオブジェクトの解析設定を保持するクラス
    /// </summary>
    public class AnalysisSetting
    {
        private readonly BasicSpeed basicSpeed;
        private readonly long minimumTiming;
        private readonly bool ignoreSpeedChangesAfterJudgeLine;
        private readonly int notesInstantiationInterval;

        public AnalysisSetting(BasicSpeed basicSpeed, long minimumTiming, bool ignoreSpeedChangesAfterJudgeLine, int notesInstantiationInterval)
        {
            if (notesInstantiationInterval < 1) throw new ArgumentOutOfRangeException(nameof(notesInstantiationInterval));
            this.basicSpeed = basicSpeed ?? throw new ArgumentNullException(nameof(basicSpeed));
            this.minimumTiming = minimumTiming;
            this.ignoreSpeedChangesAfterJudgeLine = ignoreSpeedChangesAfterJudgeLine;
            this.notesInstantiationInterval = notesInstantiationInterval;
        }

        /// <summary>
        /// ノートの基本移動速度
        /// </summary>
        public BasicSpeed BasicSpeed => basicSpeed;

        /// <summary>
        /// 譜面の最小タイミング
        /// </summary>
        public long MinimumTiming => minimumTiming;

        /// <summary>
        /// 判定ライン以降のスピード変動を無視するかどうか
        /// trueの場合、有効タイミング以降はスピード倍率が1.0に固定されます。
        /// </summary>
        public bool IgnoreSpeedChangesAfterJudgeLine => ignoreSpeedChangesAfterJudgeLine;

        /// <summary>
        /// ノート生成タイミングの間隔(ミリ秒)
        /// この値が大きいほど解析が高速になりますが、同時に存在するノート数が増加します。
        /// 推奨値：500 ~ 2000
        /// -----値による動作の比較-----
        /// 値:1000  → ノートの生成タイミングは1秒間隔になります。そのため、ノート生成タイミングはノート生成位置より後ろを表すことがあります。
        /// 値:1     → ノート生成タイミングはノート生成位置ちょうどを表すようになりますが、解析に時間がかかるようになります。
        /// </summary>
        public int NotesInstantiationInterval => notesInstantiationInterval;
    }
}
