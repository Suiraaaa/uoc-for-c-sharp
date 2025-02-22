using System;
using System.Collections.Generic;

namespace UOC.Analyze
{
    internal static class NotesDefsSectionLineTypeMapper
    {
        private const string HEADER_NOTEDEF = "NOTEDEF";
        private const string HEADER_NOTEGROUPDEF = "NOTEGROUPDEF";

        private static readonly Dictionary<string, NotesDefsSectionLineType> typeMap = new()
        {
            { HEADER_NOTEDEF, NotesDefsSectionLineType.NoteDef },
            { HEADER_NOTEGROUPDEF, NotesDefsSectionLineType.NoteGroupDef },
        };

        public static NotesDefsSectionLineType GetNotesLineType(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException(nameof(header));
            if (typeMap.TryGetValue(header, out var lineType))
            {
                return lineType;
            }
            throw new ArgumentException(nameof(header));
        }
    }
}
