using System;
using System.Collections.Generic;
using Uoc.Analyze;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートのチャンネル
    /// </summary>
    public class Channel : IEquatable<Channel>
    {
        private const int MIN_CHANNEL = 0;
        private const int MAX_CHANNEL = 1224;

        private readonly string value;
        private readonly bool isEmpty;

        private Channel()
        {
            value = string.Empty;
            isEmpty = true;
        }

        public Channel(int channel)
        {
            if (channel < MIN_CHANNEL || MAX_CHANNEL < channel) throw new ArgumentOutOfRangeException(nameof(channel));
            value = Base36.Encode(channel).PadLeft(2, '0');
            isEmpty = false;
        }

        public Channel(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));

            var channel = Base36.Decode(value);
            if (channel < MIN_CHANNEL || MAX_CHANNEL < channel) throw new ArgumentOutOfRangeException(nameof(channel));

            this.value = value;
            isEmpty = false;
        }

        public static Channel Empty => new();

        public bool IsEmpty => isEmpty;


        public int Value
        {
            get
            {
                if (isEmpty) throw new InvalidOperationException("チャンネル情報が無いため、取得できません。");
                return Base36.Decode(value);
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Channel);
        }

        public bool Equals(Channel? other)
        {
            return other is not null &&
                   value == other.value &&
                   isEmpty == other.isEmpty;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, isEmpty);
        }

        public static bool operator ==(Channel? left, Channel? right)
        {
            return EqualityComparer<Channel?>.Default.Equals(left, right);
        }

        public static bool operator !=(Channel? left, Channel? right)
        {
            return !(left == right);
        }
    }
}
