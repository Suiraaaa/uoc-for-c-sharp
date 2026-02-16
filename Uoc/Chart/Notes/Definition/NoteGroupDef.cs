using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes.Definition
{

    /// <summary>
    /// ノート定義
    /// </summary>
    public class NoteGroupDef
    {
        private readonly NoteGroupId noteGroupId;
        private readonly IReadOnlyList<NoteId> belongsNoteIds;

        public NoteGroupDef(NoteGroupId noteGroupId, IReadOnlyList<NoteId> belongsNoteIds)
        {
            this.noteGroupId = noteGroupId ?? throw new ArgumentNullException(nameof(noteGroupId));
            this.belongsNoteIds = belongsNoteIds ?? throw new ArgumentNullException(nameof(belongsNoteIds));
        }

        /// <summary>
        /// ノートグループID
        /// </summary>
        public NoteGroupId NoteGroupId => noteGroupId;

        /// <summary>
        /// グループに所属するノートIDリスト
        /// </summary>
        public IReadOnlyList<NoteId> BelongsNoteIds => belongsNoteIds;

        /// <summary>
        /// グループの始点となるノートID
        /// </summary>
        public NoteId StartNoteId => belongsNoteIds[0];

        /// <summary>
        /// グループの終点となるノートID
        /// </summary>
        public NoteId EndNoteId => belongsNoteIds[^1];
    }
}
