using System;
using UnityEngine;
using UOC.Chart;

namespace UOC.UnitySupport
{
    /// <summary>
    /// BPMをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableBPM
    {
        [SerializeField] private float bpm = 120f;

        public BPM ToBPM()
        {
            return new BPM(bpm);
        }
    }
}
