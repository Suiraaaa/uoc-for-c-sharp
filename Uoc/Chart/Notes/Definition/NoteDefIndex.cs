using System;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノート定義の番号
    /// 何番目のノート定義かを表す
    /// ----------------------------
    /// # 002501 : 0100, 3, 4
    ///      ↑ ココの部分
    /// ----------------------------
    /// </summary>
    public class NoteDefIndex
    {
        private const int MAX_INDEX = 35; // Base36の一桁で表せる最大数

        private readonly int value;

        public NoteDefIndex(int value)
        {
            if (value < 0 || MAX_INDEX < value) throw new ArgumentOutOfRangeException(nameof(value));
            this.value = value;
        }

        public int Value => value;
    }
}
