using System;

namespace UOC.Chart
{
    /// <summary>
    /// TPB（一拍の分解能）
    /// </summary>
    public class TPB
    {
        private readonly int value;

        public TPB(int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException($"TPBは0より大きい値を入力してください。(入力値: {value})");
            this.value = value;
        }

        public int Value => value;
    }
}
