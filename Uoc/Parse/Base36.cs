using System;
using System.Linq;
using System.Numerics;

namespace Uoc.Parse
{
    internal static class Base36
    {
        private const string DIGITS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static int Decode(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Empty value.");

            value = value.ToUpper();
            bool isNegative = false;
            if (value[0] == '-')
            {
                isNegative = true;
                value = value[1..];
            }
            if (value.Any(c => !DIGITS.Contains(c))) throw new ArgumentException("Invalid value: \"" + value + "\".");

            var decoded = 0;
            for (var i = 0; i < value.Length; ++i)
            {
                decoded += DIGITS.IndexOf(value[i]) * (int)BigInteger.Pow(DIGITS.Length, value.Length - i - 1);
            }

            return isNegative ? decoded * -1 : decoded;
        }

        public static string Encode(int value)
        {
            if (value == int.MinValue)
            {
                // hard coded value due to error when getting absolute value below: "Negating the minimum value of a twos complement number is invalid.".
                return "-1Y2P0IJ32E8E8";
            }

            bool isNegative = value < 0;
            value = Math.Abs(value);

            string encoded = string.Empty;
            do
            {
                encoded = DIGITS[value % DIGITS.Length] + encoded;
            }
            while ((value /= DIGITS.Length) != 0);

            return isNegative ? "-" + encoded : encoded;
        }
    }
}
