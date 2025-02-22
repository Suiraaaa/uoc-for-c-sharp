using System;
using System.Collections.Generic;

namespace UOC.Chart.Notes
{
    /// <summary>
    /// ノートグループID
    /// </summary>
    public class NoteGroupId : IEquatable<NoteGroupId>
    {
        private readonly string value;

        public NoteGroupId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            this.value = value;
        }

        public string Value => value;

        public override bool Equals(object obj)
        {
            return Equals(obj as NoteGroupId);
        }

        public bool Equals(NoteGroupId other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(NoteGroupId left, NoteGroupId right)
        {
            return EqualityComparer<NoteGroupId>.Default.Equals(left, right);
        }

        public static bool operator !=(NoteGroupId left, NoteGroupId right)
        {
            return !(left == right);
        }
    }
}
