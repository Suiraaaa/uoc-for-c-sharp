using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart.Notes.Definition
{
    /// <summary>
    /// ノートグループ定義のコレクション
    /// </summary>
    public class NoteGroupDefCollection
    {
        private readonly IReadOnlyList<NoteGroupDef> noteGroupDefs;

        public NoteGroupDefCollection(IReadOnlyList<NoteGroupDef> noteGroupDefs)
        {
            this.noteGroupDefs = noteGroupDefs ?? throw new ArgumentNullException(nameof(noteGroupDefs));
        }

        public IReadOnlyList<NoteGroupDef> NoteGroupDefs => noteGroupDefs;

        /// <summary>
        /// ノートグループ定義をノートグループIDから取得します。
        /// </summary>
        /// <param name="noteId">ノートグループID</param>
        /// <returns>指定されたノートグループIDを持つノートグループ定義</returns>
        public NoteGroupDef GetNoteGroupDefById(NoteGroupId noteGroupId)
        {
            return GetNoteGroupDefById(noteGroupId.Value);
        }

        /// <summary>
        /// ノートグループ定義をノートグループIDから取得します。
        /// </summary>
        /// <param name="noteId">ノートグループID文字列</param>
        /// <returns>指定されたノートグループIDを持つノートグループ定義</returns>
        public NoteGroupDef GetNoteGroupDefById(string noteGroupId)
        {
            if (noteGroupId == null) throw new ArgumentNullException(nameof(noteGroupId));
            var noteGroupDef = NoteGroupDefs.FirstOrDefault(x => x.NoteGroupId.Value == noteGroupId);
            return noteGroupDef ?? throw new KeyNotFoundException($"存在しないノートグループ定義にアクセスしました。(NoteId: {noteGroupId})");
        }

        /// <summary>
        /// ノートグループ定義を始点ノートIDから取得します。
        /// </summary>
        /// <param name="startNoteId">始点ノートID</param>
        /// <returns>始点ノートが指定されたノートIDを持つノートグループ定義</returns>
        public NoteGroupDef GetNoteGroupDefByStartNoteId(NoteId startNoteId)
        {
            return noteGroupDefs.FirstOrDefault(x => x.StartNoteId == startNoteId) ?? throw new KeyNotFoundException("始点ノートのIDが{\"startNoteId\"}のノートグループ定義が見つかりませんでした。");
        }

        /// <summary>
        /// 指定されたノートIDがいずれかのグループに所属しているかどうかを返します。
        /// </summary>
        /// <param name="noteId">検索対象ノートID</param>
        /// <returns>検証結果</returns>
        public bool BelongsToAnyGroup(NoteId noteId)
        {
            return BelongsToAnyGroup(noteId.Value);
        }

        /// <summary>
        /// 指定されたノートIDがいずれかのグループに所属しているかどうかを返します。
        /// </summary>
        /// <param name="noteId">検索対象ノートID</param>
        /// <returns>検証結果</returns>
        public bool BelongsToAnyGroup(string noteId)
        {
            foreach (var noteGroupDef in noteGroupDefs)
            {
                foreach (var noteIdInNoteGroup in noteGroupDef.BelongsNoteIds)
                {
                    if (noteIdInNoteGroup.Value == noteId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
