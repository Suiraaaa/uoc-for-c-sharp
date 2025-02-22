using System;

namespace UOC
{
    /// <summary>
    /// UOC文字列クラス
    /// </summary>
    public class UOCString
    {
        private readonly string uocString;

        public UOCString(string uocString)
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
