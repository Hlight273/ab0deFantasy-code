using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using HFantasy.Script.Core.Resource.IndexBuilder;
using HFantasy.Script.Configs;

namespace HFantasy.Editor.AssetBundles
{
    public static class AssetIndexBuilder
    {
        // 生成索引的菜单项
        [MenuItem("资源工具/更新资源索引")]
        public static void Build()
        {
            string directoryPath = PathConfig.ResRoot;
            string outputPath = PathConfig.AssetIndexTablePath;
            BuildIndex(directoryPath, outputPath);
        }

        // 生成索引的主要逻辑
        public static void BuildIndex(string directoryPath, string outputPath)
        {
            List<AssetIndexEntry> entries = new List<AssetIndexEntry>();
            string[] assetPaths = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);//查找指定目录下的所有资源

            foreach (string assetPath in assetPaths)
            {
                string normalizedPath = assetPath.Replace("\\", "/");
                if (normalizedPath.StartsWith("Assets/Res/ArtRes/") || normalizedPath.Contains("/ArtRes/"))//排除 Assets/Res/ArtRes 目录，这里是纯美术资源
                {
                    continue;
                }

                if (IsValidAssetType(assetPath))//只处理指定类型的资源
                {
                    AddEntries(entries, directoryPath, assetPath);
                }
            }

            AssetIndexTable table = new AssetIndexTable { Entries = entries };

            string json = JsonUtility.ToJson(table, true);
            File.WriteAllText(outputPath, json);

            Debug.Log("Asset index built and saved to: " + outputPath);
        }

        //判断资源文件是否是有效的类型
        private static bool IsValidAssetType(string assetPath)
        {
            return assetPath.EndsWith(".prefab") ||
                   assetPath.EndsWith(".mat") ||
                   assetPath.EndsWith(".ttf") ||
                   assetPath.EndsWith(".xml") ||
                   assetPath.EndsWith(".csv") ||
                   assetPath.EndsWith(".json") ||
                   assetPath.EndsWith(".mp3");
        }

        //将资源条目添加到索引表中
        private static void AddEntries(List<AssetIndexEntry> entries, string directoryPath, string assetPath)
        {
            // 统一为 Unity 风格路径
            string unityAssetPath = assetPath.Replace("\\", "/");

            // 去掉前缀 "Assets/Res/" 得到相对路径
            const string prefix = "Assets/Res/";
            if (!unityAssetPath.StartsWith(prefix))
            {
                Debug.LogWarning($"路径不合法，不在 Assets/Res 下: {unityAssetPath}");
                return;
            }

            string relativePath = unityAssetPath.Substring(prefix.Length); // e.g. "Tables/PlayerAppearanceConfig.csv"

            // 获取一级目录作为 bundleName 后缀
            string[] parts = relativePath.Split('/');
            if (parts.Length == 0)
            {
                Debug.LogWarning($"无法从路径中提取一级目录: {unityAssetPath}");
                return;
            }

            string topLevelFolder = parts[0].ToLower(); // 如 "tables"
            string bundleName = $"res/{topLevelFolder}";

            //string assetName = Path.GetFileName(unityAssetPath).ToLower(); // 如 "playerappearanceconfig.csv"
            string assetName = Path.GetFileNameWithoutExtension(unityAssetPath).ToLower(); // 如 "playerappearanceconfig.csv"

            entries.Add(new AssetIndexEntry
            {
                BundleName = bundleName,
                Key = assetName,
                AssetPath = unityAssetPath
            });
        }


    }
}
