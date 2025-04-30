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
        // ���������Ĳ˵���
        [MenuItem("��Դ����/������Դ����")]
        public static void Build()
        {
            string directoryPath = PathConfig.ResRoot;
            string outputPath = PathConfig.AssetIndexTablePath;
            BuildIndex(directoryPath, outputPath);
        }

        // ������������Ҫ�߼�
        public static void BuildIndex(string directoryPath, string outputPath)
        {
            List<AssetIndexEntry> entries = new List<AssetIndexEntry>();
            string[] assetPaths = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);//����ָ��Ŀ¼�µ�������Դ

            foreach (string assetPath in assetPaths)
            {
                string normalizedPath = assetPath.Replace("\\", "/");
                if (normalizedPath.StartsWith("Assets/Res/ArtRes/") || normalizedPath.Contains("/ArtRes/"))//�ų� Assets/Res/ArtRes Ŀ¼�������Ǵ�������Դ
                {
                    continue;
                }

                if (IsValidAssetType(assetPath))//ֻ����ָ�����͵���Դ
                {
                    AddEntries(entries, directoryPath, assetPath);
                }
            }

            AssetIndexTable table = new AssetIndexTable { Entries = entries };

            string json = JsonUtility.ToJson(table, true);
            File.WriteAllText(outputPath, json);

            Debug.Log("Asset index built and saved to: " + outputPath);
        }

        //�ж���Դ�ļ��Ƿ�����Ч������
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

        //����Դ��Ŀ��ӵ���������
        private static void AddEntries(List<AssetIndexEntry> entries, string directoryPath, string assetPath)
        {
            // ͳһΪ Unity ���·��
            string unityAssetPath = assetPath.Replace("\\", "/");

            // ȥ��ǰ׺ "Assets/Res/" �õ����·��
            const string prefix = "Assets/Res/";
            if (!unityAssetPath.StartsWith(prefix))
            {
                Debug.LogWarning($"·�����Ϸ������� Assets/Res ��: {unityAssetPath}");
                return;
            }

            string relativePath = unityAssetPath.Substring(prefix.Length); // e.g. "Tables/PlayerAppearanceConfig.csv"

            // ��ȡһ��Ŀ¼��Ϊ bundleName ��׺
            string[] parts = relativePath.Split('/');
            if (parts.Length == 0)
            {
                Debug.LogWarning($"�޷���·������ȡһ��Ŀ¼: {unityAssetPath}");
                return;
            }

            string topLevelFolder = parts[0].ToLower(); // �� "tables"
            string bundleName = $"res/{topLevelFolder}";

            //string assetName = Path.GetFileName(unityAssetPath).ToLower(); // �� "playerappearanceconfig.csv"
            string assetName = Path.GetFileNameWithoutExtension(unityAssetPath).ToLower(); // �� "playerappearanceconfig.csv"

            entries.Add(new AssetIndexEntry
            {
                BundleName = bundleName,
                Key = assetName,
                AssetPath = unityAssetPath
            });
        }


    }
}
