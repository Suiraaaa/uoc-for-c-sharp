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
        private readonly Guid guid;

        public NoteGroupProfile(NoteGroupDef noteGroupDef, IReadOnlyList<NoteProfile> belongsNotes, Guid guid)
        {
            this.noteGroupDef = noteGroupDef ?? throw new ArgumentNullException(nameof(noteGroupDef));
            this.belongsNotes = belongsNotes ?? throw new ArgumentNullException(nameof(belongsNotes));
        }

        public NoteGroupProfile(NoteGroupDef noteGroupDef, IReadOnlyList<NoteProfile> belongsNotes)
        {
            this.noteGroupDef = noteGroupDef ?? throw new ArgumentNullException(nameof(noteGroupDef));
            this.belongsNotes = belongsNotes ?? throw new ArgumentNullException(nameof(belongsNotes));
            guid = new Guid();
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
        /// ノートグループのGuid
        /// </summary>
        public Guid Guid => guid;
    }
}
