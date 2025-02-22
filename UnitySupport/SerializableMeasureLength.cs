using System;
using UnityEngine;
using UOC.Chart;

namespace UOC.UnitySupport
{
    /// <summary>
    /// MeasureLengthをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableMeasureLength
    {
        [SerializeField] private int denominator = 4;
        [SerializeField] private int numerator = 4;

        public MeasureLength ToMeasureLength()
        {
            return new MeasureLength(denominator, numerator);
        }
    }
}
