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

        [MenuItem("配置生成工具/玩家外观配置生成")]
        public static void GenerateAppearanceConfigCsv()
        {
            string exportPath = PathConfig.PlayerAppearanceExportPath;

            try
            {
                //新生成的数据
                var newList = new List<AppearanceConfigItem>();

                //旧数据读取并解析
                List<AppearanceConfigItem> oldList = new();
                if (File.Exists(exportPath))
                {
                    var csvText = File.ReadAllText(exportPath);
                    var textAsset = new TextAsset(csvText);
                    oldList = CsvUtil.ParseCsvToList<AppearanceConfigItem>(textAsset);
                }
                else//如果不存在加个默认配置
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

                //合并新旧数据（以 ResId 为主键）
                Dictionary<int, AppearanceConfigItem> mergedMap = oldList.ToDictionary(x => x.ResId);
                foreach (var newItem in newList)
                {
                    mergedMap[newItem.ResId] = newItem; //覆盖或新增
                }

                //写入合并后列表
                CsvUtil.WriteCsv(mergedMap.Values.ToList(), exportPath);

                Debug.Log($"Appearance 配置已合并导出：{exportPath}");
                AssetDatabase.Refresh();
            }
            catch (IOException ex)
            {
                if (ex.Message.ToLower().Contains("sharing violation"))
                {
                    EditorUtility.DisplayDialog("文件访问错误", "发生共享冲突，文件正在被占用或正在使用中，请关闭其他程序或检查文件权限。", "确定");
                    Debug.LogError("文件访问错误：共享冲突，无法写入文件。");
                }
                else
                {
                    Debug.LogError($"发生 IO 错误：{ex.Message}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"生成 CSV 时发生错误：{ex.Message}");
            }
        }
    }
}
