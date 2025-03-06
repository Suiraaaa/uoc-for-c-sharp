using System;

namespace Uoc.Chart
{
    /// <summary>
    /// ノートの基本移動速度
    /// </summary>
    public class BasicSpeed
    {
        private readonly float moveDuration;

        public BasicSpeed(float moveDuration)
        {
            if (moveDuration <= 0) throw new ArgumentOutOfRangeException($"入力値は0より大きい値である必要があります。");
            this.moveDuration = moveDuration;
        }

        /// <summary>
        /// ノートが生成位置から判定位置まで移動するのにかかる時間（ミリ秒単位）
        /// </summary>
        public float MoveDuration => moveDuration;
    }
}

