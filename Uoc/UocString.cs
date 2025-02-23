using System;

namespace Uoc
{
    /// <summary>
    /// UOC文字列クラス
    /// </summary>
    public class UocString
    {
        private readonly string uocString;

        public UocString(string uocString)
        {
            if (string.IsNullOrWhiteSpace(uocString)) throw new ArgumentException(nameof(uocString));
            this.uocString = uocString;
        }

        /// <summary>
        /// UOC文字列
        /// </summary>
        public string String => uocString;
    }
}
