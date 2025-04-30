using System.Collections.Generic;
namespace HFantasy.Script.Core.Resource.IndexBuilder
{
    [System.Serializable]
    public class AssetIndexTable
    {
        public List<AssetIndexEntry> Entries = new List<AssetIndexEntry>();

        public Dictionary<string, Dictionary<string, AssetIndexEntry>> ToBundleKeyMap()
        {
            var dict = new Dictionary<string, Dictionary<string, AssetIndexEntry>>();
            foreach (var entry in Entries)
            {
                if (!dict.TryGetValue(entry.BundleName, out var keyMap))
                {
                    keyMap = new Dictionary<string, AssetIndexEntry>();
                    dict[entry.BundleName] = keyMap;
                }
                keyMap[entry.Key] = entry;
            }
            return dict;
        }
    }
}
