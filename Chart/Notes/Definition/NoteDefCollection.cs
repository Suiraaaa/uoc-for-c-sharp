using System;
using System.Collections.Generic;
using System.Linq;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// ノート定義のコレクション
    /// </summary>
    public class NoteDefCollection
    {


        private readonly IReadOnlyList<NoteDef> notedefs;

        /// <summary>
        /// ノート定義のコレクションを作成します。
        /// </summary>
        /// <param name="userDefinedNoteDefs">ノート定義リスト</param>
        public NoteDefCollection(IReadOnlyList<NoteDef> notedefs)
        {
            this.notedefs = notedefs ?? throw new ArgumentNullException(nameof(notedefs));
        }

        /// <summary>
        /// ノート定義リスト
        /// </summary>
        public IReadOnlyList<NoteDef> NoteDefs => notedefs;

        /// <summary>
        /// ノート定義を番号から取得します。
        /// </summary>
        /// <param name="index">ノート定義番号</param>
        /// <returns>指定された番号に対応するノート定義</returns>
        public NoteDef GetNoteDefByIndex(NoteDefIndex index)
        {
            if (index == null) throw new ArgumentNullException(nameof(index));
            if (index.Value >= notedefs.Count) throw new InvalidOperationException($"存在しないノート定義にアクセスしました。(NoteDefIndex: {index.Value})");
            return notedefs[index.Value];
        }

        /// <summary>
        /// ノート定義をノートIDから取得します。
        /// </summary>
        /// <param name="noteId">ノートID</param>
        /// <returns>指定されたノートIDを持つノート定義</returns>
        public NoteDef GetNoteDefById(NoteId noteId)
        {
            if (noteId == null) throw new ArgumentNullException(nameof(noteId));
            var noteDef = notedefs.FirstOrDefault(x => x.NoteId == noteId);
            return noteDef ?? throw new InvalidOperationException($"存在しないノート定義にアクセスしました。(NoteId: {noteId.Value})");
        }

        /// <summary>
        /// ノート定義をノートIDから取得します。
        /// </summary>
        /// <param name="noteId">ノートID文字列</param>
        /// <returns>指定されたノートIDを持つノート定義</returns>
        public NoteDef GetNoteDefById(string noteId)
        {
            return GetNoteDefById(new NoteId(noteId));
        }

        /// <summary>
        /// ノート定義番号をノートIDから取得します。
        /// </summary>
        /// <param name="noteId">ノートID</param>
        /// <returns>指定されたノートIDを持つノート定義の番号</returns>
        public NoteDefIndex GetNoteDefIndexById(NoteId noteId)
        {
            for (int i = 0; i < notedefs.Count; i++)
            {
                if (notedefs[i].NoteId == noteId) return new NoteDefIndex(i);
            }
            throw new KeyNotFoundException(nameof(noteId));
        }
    }
}
