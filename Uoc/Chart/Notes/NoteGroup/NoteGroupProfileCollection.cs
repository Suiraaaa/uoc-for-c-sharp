using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart.Notes
{
    public class NoteGroupProfileCollection
    {
        private readonly IReadOnlyList<NoteGroupProfile> noteGroupProfiles;

        internal NoteGroupProfileCollection(IReadOnlyList<NoteGroupProfile> noteGroupProfiles)
        {
            this.noteGroupProfiles = noteGroupProfiles ?? throw new ArgumentNullException(nameof(noteGroupProfiles));
        }

        public static NoteGroupProfileCollection Create(NoteGroupDefCollection noteGroupDefCollection, NoteProfileCollection noteProfileCollection)
        {
            /* グループ内の複数ノートが同一タイミングに存在することは無いものとする。 */

            if (noteGroupDefCollection == null) throw new ArgumentNullException(nameof(noteGroupDefCollection));
            if (noteProfileCollection == null) throw new ArgumentNullException(nameof(noteProfileCollection));

            var noteGroupProfiles = new List<NoteGroupProfile>();
            var noteGroupDefs = noteGroupDefCollection.NoteGroupDefs;
            var noteProfiles = noteProfileCollection.NoteProfiles;
            int index = 0;
            while (index < noteProfiles.Count)
            {
                // 現在のノートが始点となるノートグループを探索
                var targetNoteGroupDef = noteGroupDefs.FirstOrDefault(x => noteProfiles[index].NoteDef.NoteId == x.StartNoteId);
                if (targetNoteGroupDef == null)
                {
                    index++;
                    continue;
                }

                // グループに所属するノートを抽出
                var targetChannel = noteProfiles[index].Channel;
                var belongsNotes = new List<NoteProfile>();
                for (int i = index; i < noteProfiles.Count; i++)
                {
                    if (noteProfiles[i].Channel.IsEmpty || noteProfiles[i].Channel != targetChannel)
                    {
                        continue;
                    }
                    belongsNotes.Add(noteProfiles[i]);

                    if (belongsNotes[^1].NoteDef.NoteId == targetNoteGroupDef.EndNoteId) break;
                }
                if (belongsNotes[^1].NoteDef.NoteId != targetNoteGroupDef.EndNoteId)
                {
                    throw new Exception("ノートグループの終点ノートが見つかりませんでした。");
                }

                var noteGroupProfile = new NoteGroupProfile(targetNoteGroupDef, belongsNotes);
                noteGroupProfiles.Add(noteGroupProfile);
                index++;
            }
            return new NoteGroupProfileCollection(noteGroupProfiles);
        }

        public IReadOnlyList<NoteGroupProfile> NoteGroupProfiles => noteGroupProfiles;

        /// <summary>
        /// 指定されたノートが所属しているグループの取得を試みます。
        /// </summary>
        /// <param name="note">ノート</param>
        /// <param name="noteGroup">指定されたノートが所属しているグループ（ノートがグループに所属していなかった場合はnull）</param>
        /// <returns>グループを取得できたかどうか</returns>
        public bool TryGetNoteBelongingGroup(NoteProfile note, out NoteGroupProfile noteGroup)
        {
            foreach (var noteGroupProfile in noteGroupProfiles)
            {
                if (noteGroupProfile.BelongsNotes.FirstOrDefault(x => x.Guid == note.Guid) != null)
                {
                    noteGroup = noteGroupProfile;
                    return true;
                }
            }
            noteGroup = null;
            return false;
        }
    }
}
