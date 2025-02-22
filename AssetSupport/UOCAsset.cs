using UnityEngine;

namespace UOC.AssetSupport
{
    /// <summary>
    /// UOCファイルアセット
    /// </summary>
    public class UOCAsset : ScriptableObject
    {
        [SerializeField] private string fileText;

        /// <summary>
        /// UOCファイル文字列
        /// </summary>
        public UOCString UOCString => new(fileText);

        internal void SetFileText(string fileText)
        {
            this.fileText = fileText;
        }
    }
}
