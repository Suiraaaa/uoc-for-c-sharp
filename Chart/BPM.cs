using System;

namespace UOC.Chart
{
    /// <summary>
    /// BPM
    /// </summary>
    public class BPM
    {
        private readonly float value;

        public BPM(float value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException($"BPMは0より大きい値である必要があります。(入力値: {value})");
            this.value = value;
        }

        public float Value => value;
    }
}
