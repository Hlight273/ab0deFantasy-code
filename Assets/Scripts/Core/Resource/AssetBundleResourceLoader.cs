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

        // ����AB����ͬ����
        public AssetBundle LoadAssetBundle(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
                return loadedAssetBundles[bundleName];

            // �����ǰAB�������첽���أ��ȴ��첽�������
            if (loadingTasks.ContainsKey(bundleName))
            {
                loadingTasks[bundleName].Wait();  // �ȴ��첽�������
                return loadedAssetBundles.ContainsKey(bundleName) ? loadedAssetBundles[bundleName] : null;
            }

            // ����ʼͬ������
            string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", ABPathConfig.GetPlatformFolderName(), bundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
            if (assetBundle != null)
                loadedAssetBundles[bundleName] = assetBundle;

            return assetBundle;
        }

        // ����AB�����첽��
        public async Task<AssetBundle> LoadAssetBundleAsync(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
                return loadedAssetBundles[bundleName];

            // �����AB���Ѿ��ڼ����У��ȴ���������
            if (loadingTasks.ContainsKey(bundleName))
                return await loadingTasks[bundleName];

            // �����첽��������
            string bundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundle", ABPathConfig.GetPlatformFolderName(), bundleName);
            Task<AssetBundle> loadTask = Task.Run(() =>
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
                return assetBundle;
            });

            loadingTasks[bundleName] = loadTask;

            // �ȴ�������ɣ���������ص�AB��
            AssetBundle loadedBundle = await loadTask;
            if (loadedBundle != null)
                loadedAssetBundles[bundleName] = loadedBundle;

            loadingTasks.Remove(bundleName);
            return loadedBundle;
        }

        // ���ز�ͬ���͵���Դ��ʹ�÷��ʹ���ͬ����
        public T LoadAsset<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (loadedAssets.ContainsKey(assetName))
                return loadedAssets[assetName] as T;

            AssetBundle assetBundle = LoadAssetBundle(bundleName);
            T asset = assetBundle?.LoadAsset<T>(assetName);
            loadedAssets[assetName] = asset;
            return asset;
        }

        // ���ز�ͬ���͵���Դ��ʹ�÷��ʹ����첽��
        public async Task<T> LoadAssetAsync<T>(string bundleName, string assetName) where T : UnityEngine.Object
        {
            if (loadedAssets.ContainsKey(assetName))
                return loadedAssets[assetName] as T;

            AssetBundle assetBundle = await LoadAssetBundleAsync(bundleName);
            T asset = assetBundle?.LoadAsset<T>(assetName);
            loadedAssets[assetName] = asset;
            return asset;
        }

        // ж��AB��
        public void UnloadAssetBundle(string bundleName)
        {
            if (loadedAssetBundles.ContainsKey(bundleName))
            {
                loadedAssetBundles[bundleName].Unload(true); // ж��AB����ͬʱ�ͷż��ص�������Դ
                loadedAssetBundles.Remove(bundleName);
            }
        }

        // ж�ص�����Դ
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
