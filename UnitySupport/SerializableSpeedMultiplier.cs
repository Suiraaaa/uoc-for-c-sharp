using System;
using UnityEngine;
using UOC.Chart;

namespace UOC.UnitySupport
{
    /// <summary>
    /// SpeedMultiplierをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableSpeedMultiplier
    {
        [SerializeField] private float speedMultiplier = 1f;

        public SpeedMultiplier ToSpeedMultiplier()
        {
            return new SpeedMultiplier(speedMultiplier);
        }
    }
}
