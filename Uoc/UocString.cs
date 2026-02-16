using System;

namespace Uoc
{
    /// <summary>
    /// UOC文字列を保持するクラス
    /// </summary>
    public class UocString
    {
        private readonly string value;

        public UocString(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException(nameof(value));
            this.value = value;
        }

        /// <summary>
        /// UOC文字列
        /// </summary>
        public string Value => value;
    }
}
