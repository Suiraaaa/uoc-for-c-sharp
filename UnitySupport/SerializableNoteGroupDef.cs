using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UOC.Chart.Notes;

namespace UOC.UnitySupport
{
    /// <summary>
    /// NoteGroupDefをインスペクタから設定できるようにするクラス
    /// </summary>
    [Serializable]
    public class SerializableNoteGroupDef
    {
        [SerializeField] private string noteGroupId;
        [SerializeField] private List<string> belongsNoteIds;

        public NoteGroupDef ToNoteGroupDef()
        {
            return new NoteGroupDef(new NoteGroupId(noteGroupId), belongsNoteIds.Select(x => new NoteId(x)).ToList());
        }
    }
}
