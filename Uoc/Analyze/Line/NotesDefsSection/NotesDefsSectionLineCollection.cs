using System.Collections.Generic;
using System.Linq;
using Uoc.Chart.Notes;

namespace Uoc.Analyze
{
    /// <summary>
    /// NOTESDEFSセクションに所属する行のコレクション
    /// </summary>
    internal class NotesDefsSectionLineCollection
    {
        private readonly IReadOnlyList<NotesDefsSectionLine> notesDefsSectionLines;

        private NotesDefsSectionLineCollection(IReadOnlyList<NotesDefsSectionLine> notesDefsSectionLines)
        {
            this.notesDefsSectionLines = notesDefsSectionLines;
        }

        public static NotesDefsSectionLineCollection ParseUocLines(IReadOnlyList<UocLine> lines)
        {
            var notesDefsSectionLines = lines.Select(x => NotesDefsSectionLine.ParseUocLine(x)).ToList();
            return new NotesDefsSectionLineCollection(notesDefsSectionLines);
        }

        /// <summary>
        /// 行情報からNoteDefコレクションを作成します。
        /// </summary>
        /// <returns>NoteDefコレクション</returns>
        public NoteDefCollection CreateNoteDefCollection()
        {
            var noteDefs = new List<NoteDef>();
            foreach (var line in notesDefsSectionLines)
            {
                if (line.LineType != NotesDefsSectionLineType.NoteDef) continue;
                var noteDef = line.CreateNoteDef();
                noteDefs.Add(noteDef);
            }
            return new NoteDefCollection(noteDefs);
        }

        /// <summary>
        /// 行情報からNoteGroupDefコレクションを作成します。
        /// </summary>
        public NoteGroupDefCollection CreateNoteGroupDefCollection()
        {
            var noteGroupDefs = new List<NoteGroupDef>();
            foreach (var line in notesDefsSectionLines)
            {
                if (line.LineType != NotesDefsSectionLineType.NoteGroupDef) continue;
                var noteGroupDef = line.CreateNoteGroupDef();
                noteGroupDefs.Add(noteGroupDef);
            }
            return new NoteGroupDefCollection(noteGroupDefs);
        }
    }
}
