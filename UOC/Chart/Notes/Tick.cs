using System;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// ティック
    /// </summary>
    public class Tick
    {
        private readonly int value;

        public Tick(float value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException($"Tickは0以上の値である必要があります。(入力値: {value})");
            this.value = (int)Math.Floor(value); // 小数点以下切り捨て
        }

        public int Value => value;
    }
}
