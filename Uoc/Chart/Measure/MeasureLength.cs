using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 小節長
    /// </summary>
    public class MeasureLength
    {
        private readonly int denominator;
        private readonly int numerator;

        public MeasureLength(int denominator, int numerator)
        {
            if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));
            if (numerator <= 0) throw new ArgumentOutOfRangeException(nameof(numerator));
            this.denominator = denominator;
            this.numerator = numerator;
        }

        /// <summary>
        /// 小節長分母
        /// </summary>
        public int Denominator => denominator;

        /// <summary>
        /// 小節長分子
        /// </summary>
        public int Numerator => numerator;

        /// <summary>
        /// 小節の拍数（小節内に含まれる四分音符の数）
        /// 例) 4/4 → 4, 4/2 → 2, 2/2 → 4, 2/4 → 8
        /// </summary>
        public float BeatsCount
        {
            get
            {
                return (float)numerator / denominator * 4;
            }
        }
    }
}
