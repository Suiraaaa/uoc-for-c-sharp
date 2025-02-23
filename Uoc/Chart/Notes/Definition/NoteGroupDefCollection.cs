using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart.Notes
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

        public NoteGroupDef GetNoteGroupById(NoteGroupId noteGroupId)
        {
            var noteGroupDef = noteGroupDefs.FirstOrDefault(x => noteGroupId == x.NoteGroupId);
            return noteGroupDef ?? throw new ArgumentException($"IDが\"{noteGroupId}\"のノートグループ定義は見つかりませんでした。");
        }

        public NoteGroupDef GetNoteGroupDefByStartNoteId(NoteId noteId)
        {
            return noteGroupDefs.FirstOrDefault(x => x.StartNoteId == noteId);
        }

        /// <summary>
        /// 指定されたノートIDがいずれかのグループに所属しているかどうかを返します。
        /// </summary>
        /// <param name="noteId">検索対象ノートID</param>
        /// <returns>検証結果</returns>
        public bool IsBelongAnyGroup(NoteId noteId)
        {
            foreach (var noteGroupDef in noteGroupDefs)
            {
                foreach (var noteIdInNoteGroup in noteGroupDef.BelognsNoteIds)
                {
                    if (noteIdInNoteGroup == noteId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
