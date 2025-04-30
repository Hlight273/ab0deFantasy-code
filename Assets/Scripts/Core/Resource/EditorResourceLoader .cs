#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using HFantasy.Script.Common;
using HFantasy.Script.Core.Resource.IndexBuilder;
using HFantasy.Script.Configs;
using System;

namespace HFantasy.Script.Core.Resource
{
    public class EditorResourceLoader : Singleton<EditorResourceLoader>, IResourceLoader
    {
        private Dictionary<string, Dictionary<string, AssetIndexEntry>> _index;

        public EditorResourceLoader()
        {
            LoadIndex();
        }

        private void LoadIndex()
        {
            try
            {
                string json = File.ReadAllText(PathConfig.AssetIndexTablePath);
                var table = JsonUtility.FromJson<AssetIndexTable>(json);
                _index = table.ToBundleKeyMap();
                Debug.Log("Asset index loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load asset index: {ex.Message}");
            }
        }


        public T LoadAsset<T>(string bundleName, string key) where T : UnityEngine.Object
        {
            if (_index == null || !_index.TryGetValue(bundleName, out var bundleDict))
            {
                Debug.LogWarning($"[EditorResourceLoader] Bundle not found: {bundleName}");
                return null;
            }

            if (!bundleDict.TryGetValue(key, out var entry))
            {
                Debug.LogWarning($"[EditorResourceLoader] Key not found in bundle: {key}");
                return null;
            }

            var asset = AssetDatabase.LoadAssetAtPath<T>(entry.AssetPath);
            if (asset == null)
                Debug.LogWarning($"[EditorResourceLoader] Asset not found at path: {entry.AssetPath}");

            return asset;
        }

        public async Task<T> LoadAssetAsync<T>(string bundleName, string key) where T : UnityEngine.Object
        {
            return await Task.FromResult(LoadAsset<T>(bundleName, key));
        }
    }
}
#endif
