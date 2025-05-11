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
            string bundleName = "tables";         // AB��������Ӧ�������
            string assetName = "playerappearanceconfig"; // AB��Դ����Сд����·����
            TextAsset csvAsset = ResourceLoader.LoadAsset<TextAsset>(bundleName, assetName);
            var itemList = CsvUtil.ParseCsvToList<AppearanceConfigItem>(csvAsset);

            playerAppearanceConfigDict.Clear();
            foreach (var item in itemList)
            {
                playerAppearanceConfigDict[item.ResId] = item;
            }

            Debug.Log($"PlayerAppearanceConfig ������ɣ��� {playerAppearanceConfigDict.Count} ����");
        }

        public AppearanceConfigItem GetPlayerAppearanceConfigById(int id)
        {
            playerAppearanceConfigDict.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// ����AppearanceId���ض�Ӧ����Դ
        /// </summary>
        public GameObject LoadPlayerAppearanceAsset(int appearanceId)
        {
            // ��ȡ���ã�ͨ��AppearanceId��
            AppearanceConfigItem config = GetPlayerAppearanceConfigById(appearanceId);
            if (config == null)
            {
                Debug.LogError($"�Ҳ���AppearanceConfig��ID: {appearanceId}");
                return null;
            }

            // ���ض�Ӧ��AssetBundle��Prefab
            GameObject result = ResourceLoader.LoadAsset<GameObject>(config.BundleName, config.AssetName);
            if (gameObject == null)
            {
                Debug.LogError($"������Դʧ��,bundle:{config.BundleName},asset:{config.AssetName}");
                return null;
            }
            else
            {
                return result;
            }
        }
    }
}
