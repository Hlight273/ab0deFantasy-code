using UnityEditor;
using UnityEngine;
using System.IO;

namespace HFantasy.Editor.AssetBundles
{
    public static class AssetBundleLabelTool
    {
        private const string TargetRoot = "Assets/Res/Characters";//prefabs的资源目录
        private const string AssetBundlePrefix = ""; //bundle前缀
        private const string CharacterBundleName = "characters";

        private const string TableRoot = "Assets/Res/Tables";
        private const string TableBundleName = "tables";

        /// <summary>
        /// 找Assets/Prefabs中的所有prefab，然后找到其父级目录Prefabs目录的子目录名作为bundlename，其父级目录Prefabs目录的子目录的子目录名作为assetname
        /// </summary>
        [MenuItem("资源工具/设置所有Characters的AssetBundle标签")]
        public static void SetPrefabAssetBundleLabels()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { TargetRoot });
            int count = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string relativePath = path.Replace(TargetRoot + "/", ""); //去掉前缀
                string[] split = relativePath.Split('/');

                if (split.Length < 2)
                {
                    Debug.LogWarning($"跳过：{path}（不满足 Prefabs/<一级>/<二级>/xxx.prefab 格式）");
                    continue;
                }

                string bundleName = CharacterBundleName;//AssetBundlePrefix + split[0].ToLower();
                string assetName = split[1].ToLower();

                AssetImporter importer = AssetImporter.GetAtPath(path);
                if (importer != null)
                {
                    importer.assetBundleName = bundleName;
                   // importer.assetBundleVariant = assetName;
                    count++;
                    Debug.Log($"? 设置：{path} -> BundleName: {bundleName}");
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("设置完成", $"共设置 {count} 个 Characters 的 AssetBundle 标签", "OK");
        }

        [MenuItem("资源工具/设置所有CSV配置表的AssetBundle标签")]
        public static void SetCsvAssetBundleLabels()
        {
            string[] csvPaths = Directory.GetFiles(TableRoot, "*.csv", SearchOption.AllDirectories);
            int count = 0;

            foreach (string path in csvPaths)
            {
                string assetPath = path.Replace("\\", "/"); // 兼容Windows路径
                AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                if (importer != null)
                {
                    importer.assetBundleName = TableBundleName;
                    importer.assetBundleVariant = null;
                    count++;
                    Debug.Log($"设置表格：{assetPath} -> BundleName: {TableBundleName}");
                }
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("设置完成", $"共设置 {count} 个 CSV 表格的 AssetBundle 标签", "OK");
        }
    }

}
