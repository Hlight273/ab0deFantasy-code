using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace HFantasy.Script.Core.Resource
{
    public class ABInitializer : MonoBehaviour
    {
        public static event System.Action OnABInitialized;
        public static bool IsInitialized { get; private set; }

        private string streamingABPath;
        private string persistentABPath;

        void Start()
        {
#if !UNITY_ANDROID
            return;
#else
            streamingABPath = Path.Combine(Application.streamingAssetsPath, "AssetBundle");
            persistentABPath = Path.Combine(Application.persistentDataPath, "AssetBundle");

            StartCoroutine(CopyABFilesIfNeeded());
#endif
        }

        private IEnumerator CopyABFilesIfNeeded()
        {
            if (!Directory.Exists(persistentABPath))
            {
                Directory.CreateDirectory(persistentABPath);
            }
            if (Directory.Exists(persistentABPath) && Directory.GetFiles(persistentABPath).Length > 0)
            {
                Debug.Log("AB资源已存在，无需拷贝");
                IsInitialized = true;
                OnABInitialized?.Invoke();
                yield break;
            }

#if UNITY_ANDROID
             Debug.Log("Android平台，开始拷贝AB包...");

            string abRoot = "AssetBundle/Android";
            string manifestFilePath = $"jar:file://{Application.dataPath}!/assets/{abRoot}/Android";
            Debug.Log("主AB包文件路径: " + manifestFilePath);

            using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(manifestFilePath))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"主AB包文件加载失败：{www.error}，路径：{manifestFilePath}");
                    yield break;
                }

                Debug.Log("主AB包文件下载成功，准备加载...");
                AssetBundle manifestAB = DownloadHandlerAssetBundle.GetContent(www);
                
                if (manifestAB == null)
                {
                    Debug.LogError("主AB包加载失败");
                    yield break;
                }

                // 获取所有资源名称进行调试
                string[] assetNames = manifestAB.GetAllAssetNames();
                Debug.Log($"主AB包内资源列表：");
                foreach (var name in assetNames)
                {
                    Debug.Log($"- {name}");
                }

                AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                {
                    Debug.LogError("解析 AssetBundleManifest 失败！");
                    manifestAB.Unload(false);
                    yield break;
                }

                //对于其他AB包，也使用AssetBundle方式加载
                string[] abNames = manifest.GetAllAssetBundles();
                foreach (string abName in abNames)
                {
                    //移除路径中多余的"Res/"前缀，因为AB包的bundlename是Res/xxx 但是安卓都会变成大写路径
                    //string fileName = abName.Replace("Res/", "");
                    string abFilePath = $"jar:file://{Application.dataPath}!/assets/{abRoot}/{abName}";
                    string destPath = Path.Combine(persistentABPath, abName);

                    Debug.Log($"准备拷贝: {abName}，路径：{abFilePath}");

                    using (UnityWebRequest abRequest = UnityWebRequest.Get(abFilePath))  // 改用普通Get请求
                    {
                        yield return abRequest.SendWebRequest();

                        if (abRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError($"AB包加载失败: {abFilePath} - {abRequest.error}");
                            continue;
                        }

                        // 直接获取下载的原始数据
                        byte[] abData = abRequest.downloadHandler.data;
                        if (abData != null)
                        {
                            File.WriteAllBytes(destPath, abData);
                            Debug.Log($"成功拷贝AB包: {abName} 到 {destPath}");
                        }
                    }
                }

                manifestAB.Unload(false);
            }
#else
            // Windows, iOS, macOS, 编辑器环境
            if (Directory.Exists(streamingABPath))
            {
                if (!Directory.Exists(persistentABPath))
                    Directory.CreateDirectory(persistentABPath);

                string[] files = Directory.GetFiles(streamingABPath);

                foreach (var file in files)
                {
                    string name = Path.GetFileName(file);
                    if (name.EndsWith(".manifest") || name.EndsWith(".meta")) continue;

                    string destPath = Path.Combine(persistentABPath, name);
                    File.Copy(file, destPath, true);
                    Debug.Log("拷贝完成: " + name);
                }
            }
            else
            {
                Debug.LogWarning("StreamingAssets 路径不存在: " + streamingABPath);
            }
#endif

            IsInitialized = true;
            OnABInitialized?.Invoke();
            Debug.Log("首次拷贝 AB 包完成！");
            Destroy(gameObject);
        }
    }
}
