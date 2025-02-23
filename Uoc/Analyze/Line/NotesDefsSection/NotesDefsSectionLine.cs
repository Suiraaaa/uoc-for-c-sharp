using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Uoc.Chart.Notes;

namespace Uoc.Analyze
{
    /// <summary>
    /// NOTESDEFSセクションに所属する行
    /// </summary>
    internal class NotesDefsSectionLine
    {
        private readonly string definitionId;
        private readonly NotesDefsSectionLineType lineType;
        private readonly IReadOnlyList<string> lineProperties;

        public NotesDefsSectionLine(string definitionId, NotesDefsSectionLineType lineType, IReadOnlyList<string> lineProperties)
        {
            if (string.IsNullOrWhiteSpace(definitionId)) throw new ArgumentException(nameof(definitionId));
            if (definitionId.Contains(' ')) throw new ArgumentException($"定義IDに空白を含めることはできません。(入力値: {definitionId})");
            if (lineProperties == null) throw new ArgumentNullException(nameof(lineProperties));

            this.definitionId = definitionId;
            this.lineType = lineType;
            this.lineProperties = lineProperties;
        }

        public static NotesDefsSectionLine ParseUocLine(UocLine line)
        {
            try
            {
                /*
                 * 想定入力形式例
                 * NOTEDEF "Tap", "x", "size"
                 * NOTEGROUPDEF "Hold", "HoldStart", "HoldEnd"
                 */
                var match = Regex.Match(line.LineText, @"^(NOTEDEF|NOTEGROUPDEF).*$");
                if (!match.Success)
                {
                    throw new Exception("正規表現によるパースに失敗しました。");
                }
                var lineType = NotesDefsSectionLineTypeMapper.GetNotesLineType(match.Groups[1].Value);

                var valueMatches = Regex.Matches(line.LineText, @"\""(.*?)\""");
                foreach (var valueMatch in valueMatches.Cast<Match>())
                {
                    if (!valueMatch.Success)
                    {
                        throw new Exception("正規表現によるパースに失敗しました。");
                    }
                }
                var definitionId = valueMatches[0].Groups[1].Value;
                var lineProperties = new List<string>();
                for (int i = 1; i < valueMatches.Count; i++)
                {
                    lineProperties.Add(valueMatches[i].Groups[1].Value);
                }

                return new NotesDefsSectionLine(definitionId, lineType, lineProperties);
            }
            catch (Exception e)
            {
                throw new Exception($"行{line}をNOTESDEFSセクション行にパースできませんでした。", e);
            }

        }

        /// <summary>
        /// 行種別
        /// </summary>
        public NotesDefsSectionLineType LineType => lineType;

        public NoteDef CreateNoteDef()
        {
            if (lineType != NotesDefsSectionLineType.NoteDef) throw new InvalidOperationException("行はNoteDef定義ではないため、NoteDefを作成できません。");
            var noteId = new NoteId(definitionId);
            return new NoteDef(noteId, lineProperties);
        }

        public NoteGroupDef CreateNoteGroupDef()
        {
            if (lineType != NotesDefsSectionLineType.NoteGroupDef) throw new InvalidOperationException("行はNoteGroupDef定義ではないため、NoteGroupDefを作成できません。");
            var noteGroupId = new NoteGroupId(definitionId);
            var noteIds = lineProperties.Select(x => new NoteId(x)).ToList();
            return new NoteGroupDef(noteGroupId, noteIds);
        }
    }
}
