#if UNITY_EDITOR

using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UOC.AssetSupport
{
    [ScriptedImporter(1, "uoc")]
    internal class UOCAssetImporter : ScriptedImporter
    {
        private const string ICON_PATH = @"UOCAssetIcon";

        public override void OnImportAsset(AssetImportContext ctx)
        {
            UOCAsset uocAsset = ScriptableObject.CreateInstance<UOCAsset>();
            uocAsset.SetFileText(File.ReadAllText(ctx.assetPath));

            ctx.AddObjectToAsset("UOCAsset", uocAsset, GetUOCAssetIconTexture());
        }

        private Texture2D GetUOCAssetIconTexture()
        {
            return Resources.Load<Texture2D>(ICON_PATH);
        }
    }
}

#endif
