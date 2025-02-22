using System;
using System.Collections.Generic;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// グループの再生を提供するクラス
    /// </summary>
    public class NoteGroupPlaybackProvider
    {
        private readonly NoteGroupId noteGroupId;
        private readonly IReadOnlyList<NotePlaybackProvider> notePlaybackProviders;

        internal NoteGroupPlaybackProvider(NoteGroupId noteGroupId, IReadOnlyList<NotePlaybackProvider> notePlaybackProviders)
        {
            if (noteGroupId == null) throw new ArgumentNullException(nameof(noteGroupId));
            if (notePlaybackProviders == null) throw new ArgumentNullException(nameof(notePlaybackProviders));
            if (notePlaybackProviders.Count == 0) throw new ArgumentNullException("グループに所属するノートが含まれていません。");

            this.noteGroupId = noteGroupId;
            this.notePlaybackProviders = notePlaybackProviders;
        }

        /// <summary>
        /// ノートグループID
        /// </summary>
        public string NoteGroupId => noteGroupId.Value;

        /// <summary>
        /// グループに所属するノートのリスト
        /// </summary>
        public IReadOnlyList<NotePlaybackProvider> BelongsNotes => notePlaybackProviders;

        /// <summary>
        /// グループ始点の生成タイミング
        /// </summary>
        public long FirstInstantiateTiming => notePlaybackProviders[0].InstantiateTiming;
    }
}
