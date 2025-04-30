using HFantasy.Script.Common;
using HFantasy.Script.Configs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace HFantasy.Script.Core.Resource
{
    public class AssetBundleResourceLoader: Singleton<AssetBundleResourceLoader>, IResourceLoader
    {
        private Dictionary<string, AssetBundle> loadedAssetBundles = new Dictionary<string, AssetBundle>();
        private Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();
        private Dictionary<string, Task<AssetBundle>> loadingTasks = new Dictionary<string, Task<AssetBundle>>();

        // 加载AB包（同步）
        public AssetBundle LoadAssetBundle(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
                return loadedAssetBundles[bundleName];

            // 如果当前AB包正在异步加载，等待异步加载完成
            if (loadingTasks.ContainsKey(bundleName))
            {
                loadingTasks[bundleName].Wait();  // 等待异步加载完成
                return loadedAssetBundles.ContainsKey(bundleName) ? loadedAssetBundles[bundleName] : null;
            }

            // 否则开始同步加载
            string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", ABPathConfig.GetPlatformFolderName(), bundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle != null)
                loadedAssetBundles[bundleName] = assetBundle;

            return assetBundle;
        }

        // 加载AB包（异步）
        public async Task<AssetBundle> LoadAssetBundleAsync(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
                return loadedAssetBundles[bundleName];

            // 如果该AB包已经在加载中，等待其加载完成
            if (loadingTasks.ContainsKey(bundleName))
                return await loadingTasks[bundleName];

            // 启动异步加载任务
            string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", ABPathConfig.GetPlatformFolderName(), bundleName);
            Task<AssetBundle> loadTask = Task.Run(() =>
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
                return assetBundle;
            });

            loadingTasks[bundleName] = loadTask;

            // 等待任务完成，并缓存加载的AB包
            AssetBundle loadedBundle = await loadTask;
            if (loadedBundle != null)
                loadedAssetBundles[bundleName] = loadedBundle;

            loadingTasks.Remove(bundleName);
            return loadedBundle;
        }

        // 加载不同类型的资源，使用泛型处理（同步）
        public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (loadedAssets.ContainsKey(assetName))
                return loadedAssets[assetName] as T;

            AssetBundle assetBundle = LoadAssetBundle(bundleName);
            T asset = assetBundle?.LoadAsset<T>(assetName);
            loadedAssets[assetName] = asset;
            return asset;
        }

        // 加载不同类型的资源，使用泛型处理（异步）
        public async Task<T> LoadAssetAsync<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (loadedAssets.ContainsKey(assetName))
                return loadedAssets[assetName] as T;

            AssetBundle assetBundle = await LoadAssetBundleAsync(bundleName);
            T asset = assetBundle?.LoadAsset<T>(assetName);
            loadedAssets[assetName] = asset;
            return asset;
        }

        // 卸载AB包
        public void UnloadAssetBundle(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
            {
                loadedAssetBundles[bundleName].Unload(true); // 卸载AB包的同时释放加载的所有资源
                loadedAssetBundles.Remove(bundleName);
            }
        }

        // 卸载单个资源
        public void UnloadAsset(string assetName)
        {
            if (loadedAssets.ContainsKey(assetName))
            {
                Resources.UnloadAsset(loadedAssets[assetName]);
                loadedAssets.Remove(assetName);
            }
        }
    }
}
