using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートID
    /// </summary>
    public class NoteId : IEquatable<NoteId>
    {
        private readonly string value;

        public NoteId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            this.value = value;
        }

        public string Value => value;

        public override bool Equals(object obj)
        {
            return Equals(obj as NoteId);
        }

        public bool Equals(NoteId? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(NoteId? left, NoteId? right)
        {
            return EqualityComparer<NoteId?>.Default.Equals(left, right);
        }

        public static bool operator !=(NoteId? left, NoteId? right)
        {
            return !(left == right);
        }
    }
}
