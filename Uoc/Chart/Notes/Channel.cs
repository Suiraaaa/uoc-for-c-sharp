using System;
using System.Collections.Generic;

namespace Uoc.Chart.Notes
{
    /// <summary>
    /// ノートのチャンネル
    /// </summary>
    public class Channel : IEquatable<Channel?>
    {
        private const int MIN_CHANNEL = 0;
        private const int MAX_CHANNEL = 1224;

        private readonly int? value;

        private Channel()
        {
            value = null;
        }

        public Channel(int value)
        {
            if (value < MIN_CHANNEL || MAX_CHANNEL < value) throw new ArgumentOutOfRangeException($"チャンネルは{MIN_CHANNEL}から{MAX_CHANNEL}の範囲内である必要があります。(入力値: {value})");
            this.value = value;
        }

        public static Channel Empty => new();

        public bool IsEmpty => value != null;


        public int Value
        {
            get
            {
                if (value == null) throw new InvalidOperationException("チャンネル情報が無いため、取得できません。");
                return (int)value;
            }
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Channel);
        }

        public bool Equals(Channel? other)
        {
            return other is not null &&
                   value == other.value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value);
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
