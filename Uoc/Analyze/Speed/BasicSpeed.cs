using System;
using System.Collections.Generic;

namespace Uoc.Analyze.Speed
{
    /// <summary>
    /// ノートの基本移動速度
    /// </summary>
    public class BasicSpeed : IEquatable<BasicSpeed>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as BasicSpeed);
        }

        public bool Equals(BasicSpeed? other)
        {
            return other is not null &&
                   moveDuration == other.moveDuration;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(moveDuration);
        }

        public static bool operator ==(BasicSpeed left, BasicSpeed right)
        {
            return EqualityComparer<BasicSpeed>.Default.Equals(left, right);
        }

        public static bool operator !=(BasicSpeed left, BasicSpeed right)
        {
            return !(left == right);
        }
    }
}

