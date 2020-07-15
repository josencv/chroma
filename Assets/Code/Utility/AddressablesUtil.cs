using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.AddressableAssets;

namespace Chroma.Utility
{
    public static class AddressablesUtil
    {
        public static AssetReference MarkAssetAsAddressable(string assetPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.DefaultGroup;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
            entry.address = assetPath;
            entry.ReadOnly = true;
            entry.labels.Add("ProbeData");
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

            return new AssetReference(entry.guid);
        }
    }
}
