using System;

namespace Uoc.Chart
{
    /// <summary>
    /// 小節長を表すクラス
    /// 分子は小節内の拍数、分母は1拍に相当する音符の種類を示す。
    /// </summary>
    public class MeasureLength
    {
        private readonly int numerator;
        private readonly int denominator;

        public MeasureLength(int numerator, int denominator)
        {
            if (numerator <= 0) throw new ArgumentOutOfRangeException(nameof(numerator));
            if (denominator <= 0) throw new ArgumentOutOfRangeException(nameof(denominator));
            this.numerator = numerator;
            this.denominator = denominator;
        }

        /// <summary>
        /// 小節内の拍数（例: 4/4拍子なら4）
        /// </summary>
        public int Numerator => numerator;

        /// <summary>
        /// 1拍として扱う音符の種類（例: 4/4拍子なら4、4/2拍子なら2）
        /// </summary>
        public int Denominator => denominator;

        /// <summary>
        /// 小節内の拍数を返す。
        /// 例: 4/4拍子なら4、2/4拍子なら2
        /// </summary>
        /// <returns>小節内の拍数</returns>
        public int GetBeatCount()
        {
            return numerator;
        }

        /// <summary>
        /// 小節内の四分音符換算の拍数を返す。
        /// 例:
        ///   - 4/4拍子: 4 * (4/4) = 4
        ///   - 4/2拍子: 4 * (4/2) = 8
        ///   - 2/4拍子: 2 * (4/4) = 2
        ///   - 2/2拍子: 2 * (4/2) = 4
        /// </summary>
        /// <returns>四分音符換算の拍数</returns>
        public float GetQuarterNoteCount()
        {
            return numerator * (4f / denominator);
        }
    }
}
