using System;
using UnityEngine;
using UOC.Chart;

namespace UOC.UnitySupport
{
    /// <summary>
    /// Layerをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableLayer
    {
        [SerializeField] private int layer = 0;

        public Layer ToLayer()
        {
            return new Layer(layer);
        }
    }
}
