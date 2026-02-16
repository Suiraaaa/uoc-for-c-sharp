using System;
using System.Collections.Generic;
using Uoc.Chart.Notes.Definition;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートグループを構成する情報を持つクラス
    /// </summary>
    public class NoteGroupProfile
    {
        private readonly NoteGroupDef noteGroupDef;
        private readonly IReadOnlyList<NoteProfile> belongsNotes;
        private readonly NoteGuid guid;

        public NoteGroupProfile(NoteGroupDef noteGroupDef, IReadOnlyList<NoteProfile> belongsNotes, NoteGuid guid)
        {
            this.noteGroupDef = noteGroupDef ?? throw new ArgumentNullException(nameof(noteGroupDef));
            this.belongsNotes = belongsNotes ?? throw new ArgumentNullException(nameof(belongsNotes));
            this.guid = guid ?? throw new ArgumentNullException(nameof(guid));
        }

        public NoteGroupProfile(NoteGroupDef noteGroupDef, IReadOnlyList<NoteProfile> belongsNotes)
        {
            this.noteGroupDef = noteGroupDef ?? throw new ArgumentNullException(nameof(noteGroupDef));
            this.belongsNotes = belongsNotes ?? throw new ArgumentNullException(nameof(belongsNotes));
            guid = new NoteGuid();
        }

        /// <summary>
        /// ノートグループ定義
        /// </summary>
        public NoteGroupDef NoteGroupDef => noteGroupDef;

        /// <summary>
        /// 所属するノーツ
        /// </summary>
        public IReadOnlyList<NoteProfile> BelongsNotes => belongsNotes;

        /// <summary>
        /// Guid
        /// </summary>
        public NoteGuid Guid => guid;
    }
}
