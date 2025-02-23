using System;

namespace Uoc.Chart
{
    /// <summary>
    /// BPM
    /// </summary>
    public class Bpm
    {
        private readonly float value;

        public Bpm(float value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException($"BPMは0より大きい値である必要があります。(入力値: {value})");
            this.value = value;
        }

        public float Value => value;
    }
}
