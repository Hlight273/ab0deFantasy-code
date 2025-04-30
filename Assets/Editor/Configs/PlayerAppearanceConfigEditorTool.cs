using HFantasy.Script.Common.Utils;
using HFantasy.Script.Configs;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HFantasy.Editor.Configs
{
    public static class PlayerAppearanceConfigEditorTool
    {

        [MenuItem("�������ɹ���/��������������")]
        public static void GenerateAppearanceConfigCsv()
        {
            string exportPath = PathConfig.PlayerAppearanceExportPath;

            try
            {
                //�����ɵ�����
                var newList = new List<AppearanceConfigItem>();

                //�����ݶ�ȡ������
                List<AppearanceConfigItem> oldList = new();
                if (File.Exists(exportPath))
                {
                    var csvText = File.ReadAllText(exportPath);
                    var textAsset = new TextAsset(csvText);
                    oldList = CsvUtil.ParseCsvToList<AppearanceConfigItem>(textAsset);
                }
                else//��������ڼӸ�Ĭ������
                {
                    AppearanceConfigItem appearanceConfigItem = new AppearanceConfigItem
                    {
                        ResId = 1,
                        Name = "Girl1_Body",
                        PartType = AppearancePartType.Body,
                        PartTypeId = 0,
                        BundleName = "res/characters",
                        AssetName = "girl1"
                    };
                    newList.Add(appearanceConfigItem);
                }

                //�ϲ��¾����ݣ��� ResId Ϊ������
                Dictionary<int, AppearanceConfigItem> mergedMap = oldList.ToDictionary(x => x.ResId);
                foreach (var newItem in newList)
                {
                    mergedMap[newItem.ResId] = newItem; //���ǻ�����
                }

                //д��ϲ����б�
                CsvUtil.WriteCsv(mergedMap.Values.ToList(), exportPath);

                Debug.Log($"Appearance �����Ѻϲ�������{exportPath}");
                AssetDatabase.Refresh();
            }
            catch (IOException ex)
            {
                if (ex.Message.ToLower().Contains("sharing violation"))
                {
                    EditorUtility.DisplayDialog("�ļ����ʴ���", "���������ͻ���ļ����ڱ�ռ�û�����ʹ���У���ر�������������ļ�Ȩ�ޡ�", "ȷ��");
                    Debug.LogError("�ļ����ʴ��󣺹����ͻ���޷�д���ļ���");
                }
                else
                {
                    Debug.LogError($"���� IO ����{ex.Message}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"���� CSV ʱ��������{ex.Message}");
            }
        }
    }
}
