using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes
{

    /// <summary>
    /// ノート定義
    /// </summary>
    public class NoteGroupDef
    {
        private readonly NoteGroupId noteGroupId;
        private readonly IReadOnlyList<NoteId> belognsNoteIds;

        public NoteGroupDef(NoteGroupId noteGroupId, IReadOnlyList<NoteId> belognsNoteIds)
        {
            this.noteGroupId = noteGroupId ?? throw new ArgumentNullException(nameof(noteGroupId));
            this.belognsNoteIds = belognsNoteIds ?? throw new ArgumentNullException(nameof(belognsNoteIds));
        }

        /// <summary>
        /// ノートグループID
        /// </summary>
        public NoteGroupId NoteGroupId => noteGroupId;

        /// <summary>
        /// グループに所属するノートIDリスト
        /// </summary>
        public IReadOnlyList<NoteId> BelognsNoteIds => belognsNoteIds;

        /// <summary>
        /// グループの始点となるノートID
        /// </summary>
        public NoteId StartNoteId => belognsNoteIds[0];

        /// <summary>
        /// グループの終点となるノートID
        /// </summary>
        public NoteId EndNoteId => belognsNoteIds[^1];
    }
}
