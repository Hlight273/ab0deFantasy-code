using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace HFantasy.Script.Core.Resource
{
    public class ABInitializer : MonoBehaviour
    {
        private string streamingABPath;
        private string persistentABPath;

        void Start()
        {
            streamingABPath = Path.Combine(Application.streamingAssetsPath, "AssetBundle");
            persistentABPath = Path.Combine(Application.persistentDataPath, "AssetBundle");

            StartCoroutine(CopyABFilesIfNeeded());
        }

        private IEnumerator CopyABFilesIfNeeded()
        {
            if (Directory.Exists(persistentABPath) && Directory.GetFiles(persistentABPath).Length > 0)
            {
                Debug.Log("AB资源已存在，无需拷贝");
                yield break;
            }

            Debug.Log("正在首次拷贝 AB 包...");

#if UNITY_ANDROID && !UNITY_EDITOR
            // Android 需使用 UnityWebRequest 访问 jar:file:// 路径
            string manifestPath = streamingABPath + "/AssetBundle"; // 主包用于枚举
            UnityWebRequest manifestRequest = UnityWebRequestAssetBundle.GetAssetBundle(manifestPath);
            yield return manifestRequest.SendWebRequest();

            if (manifestRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("获取 AB 清单失败: " + manifestRequest.error);
                yield break;
            }

            // 可在 manifest 中手动记录所有资源列表，或这里写死
            string[] bundlesToCopy = new string[] { "AssetBundle" }; // 自行补全或从远程拉取配置

            foreach (var bundleName in bundlesToCopy)
            {
                string srcPath = streamingABPath + "/" + bundleName;
                string destPath = Path.Combine(persistentABPath, bundleName);

                UnityWebRequest request = UnityWebRequest.Get(srcPath);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("拷贝失败: " + bundleName + " -> " + request.error);
                    continue;
                }

                if (!Directory.Exists(persistentABPath))
                    Directory.CreateDirectory(persistentABPath);

                File.WriteAllBytes(destPath, request.downloadHandler.data);
                Debug.Log("拷贝完成: " + bundleName);
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

            Debug.Log("首次拷贝 AB 包完成！");
            Destroy(gameObject);
        }
    }
}