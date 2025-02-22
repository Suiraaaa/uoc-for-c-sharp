using System;
using System.Collections.Generic;
using UnityEngine;
using UOC.Chart.Notes;

namespace UOC.UnitySupport
{
    /// <summary>
    /// NoteDefをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableNoteDef
    {
        [SerializeField] private string noteId;
        [SerializeField] private List<string> propertyNames;

        public NoteDef ToNoteDef()
        {
            return new NoteDef(new NoteId(noteId), propertyNames);
        }
    }
}
