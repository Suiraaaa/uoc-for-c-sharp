using System;
using System.Collections.Generic;
using System.Linq;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノート定義
    /// </summary>
    public class NoteDef
    {
        private static readonly IReadOnlyList<NoteDef> commonNoteDefs = new List<NoteDef>()
        {
            new NoteDef(
                new NoteId("BPMChange"),
                new List<string>() { "bpm" }
            ),
            new NoteDef(
                new NoteId("SpeedChange"),
                new List<string>() { "speed" }
            ),
            new NoteDef(
                new NoteId("MeasureLengthChange"),
                new List<string>() { "denominator", "numerator" }
            )
        };

        private readonly NoteId noteId;
        private readonly IReadOnlyList<string> propertyNames;

        public NoteDef(NoteId noteId, IReadOnlyList<string> propertyNames)
        {
            this.noteId = noteId ?? throw new ArgumentNullException(nameof(noteId));
            this.propertyNames = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
        }

        public static NoteDef BpmChange => commonNoteDefs.First(x => x.NoteId.Value == "BPMChange");

        public static NoteDef SpeedChange => commonNoteDefs.First(x => x.NoteId.Value == "SpeedChange");

        public static NoteDef MeasureLengthChange => commonNoteDefs.First(x => x.NoteId.Value == "MeasureLengthChange");

        /// <summary>
        /// ノートID
        /// </summary>
        public NoteId NoteId => noteId;

        /// <summary>
        /// ノートプロパティ名のリスト
        /// </summary>
        public IReadOnlyList<string> PropertyNames => propertyNames;

        /// <summary>
        /// 仕様で定義されたノート定義リスト
        /// </summary>
        public static IReadOnlyList<NoteDef> CommonNoteDefs => commonNoteDefs;
    }
}
