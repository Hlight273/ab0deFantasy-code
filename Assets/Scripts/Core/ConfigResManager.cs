using HFantasy.Script.Common;
using HFantasy.Script.Common.Utils;
using HFantasy.Script.Core.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Configs
{
    public class ConfigResManager : MonoSingleton<ConfigResManager>
    {
        private Dictionary<int, AppearanceConfigItem> playerAppearanceConfigDict = new Dictionary<int, AppearanceConfigItem>();

        public void LoadPlayerAppearanceConfig()
        {
            //string path = PathConfig.PlayerAppearanceExportPath;
            string bundleName = "tables";         // AB包名（对应打包规则）
            string assetName = "playerappearanceconfig"; // AB资源名（小写、无路径）
            TextAsset csvAsset = ResourceLoader.LoadAsset<TextAsset>(bundleName, assetName);
            var itemList = CsvUtil.ParseCsvToList<AppearanceConfigItem>(csvAsset);

            playerAppearanceConfigDict.Clear();
            foreach (var item in itemList)
            {
                playerAppearanceConfigDict[item.ResId] = item;
            }

            Debug.Log($"PlayerAppearanceConfig 加载完成，共 {playerAppearanceConfigDict.Count} 条！");
        }

        public AppearanceConfigItem GetPlayerAppearanceConfigById(int id)
        {
            playerAppearanceConfigDict.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// 根据AppearanceId加载对应的资源
        /// </summary>
        public GameObject LoadPlayerAppearanceAsset(int appearanceId)
        {
            // 获取配置（通过AppearanceId）
            AppearanceConfigItem config = GetPlayerAppearanceConfigById(appearanceId);
            if (config == null)
            {
                Debug.LogError($"找不到AppearanceConfig，ID: {appearanceId}");
                return null;
            }

            // 加载对应的AssetBundle和Prefab
            GameObject result = ResourceLoader.LoadAsset<GameObject>(config.BundleName, config.AssetName);
            if (gameObject == null)
            {
                Debug.LogError($"加载资源失败,bundle:{config.BundleName},asset:{config.AssetName}");
                return null;
            }
            else
            {
                return result;
            }
        }
    }
}
