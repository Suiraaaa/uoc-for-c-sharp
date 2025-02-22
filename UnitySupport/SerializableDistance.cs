using System;
using UnityEngine;
using UOC.Chart;

namespace UOC.UnitySupport
{
    /// <summary>
    /// Layerをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableDistance
    {
        [SerializeField] private float beatsCount = 0;

        public Distance ToDistance()
        {
            return new Distance(beatsCount);
        }
    }
}
