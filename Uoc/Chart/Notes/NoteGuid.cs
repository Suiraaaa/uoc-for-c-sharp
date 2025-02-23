using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートのGUID
    /// </summary>
    public class NoteGuid : IEquatable<NoteGuid>
    {
        private readonly string value;

        public NoteGuid()
        {
            value = Guid.NewGuid().ToString();
        }

        public string Value => value;

        public override bool Equals(object obj)
        {
            return Equals(obj as NoteGuid);
        }

        public bool Equals(NoteGuid? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
        }

        public static bool operator ==(NoteGuid? left, NoteGuid? right)
        {
            return EqualityComparer<NoteGuid?>.Default.Equals(left, right);
        }

        public static bool operator !=(NoteGuid? left, NoteGuid? right)
        {
            return !(left == right);
        }
    }
}
