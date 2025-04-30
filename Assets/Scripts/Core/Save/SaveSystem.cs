using HFantasy.Script.Configs;
using HFantasy.Script.Entity;
using HFantasy.Script.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HFantasy.Script.Core.Save
{
    public class SaveSystem
    {
        //private static string saveFolder => Application.persistentDataPath + PathConfig.SaveFolder;
        private static string saveFolder => PathConfig.SaveFolder;

        public static bool CreateNewSave(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";

            // 防止覆盖已有存档
            if (File.Exists(path))
                return false;

            Save(saveFileName, BuildDefaultSaveData());
            return true;
        }

        public static void Save(string saveFileName, SaveData data)
        {
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFolder + saveFileName + ".json", json);
        }

        public static SaveData Load(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(json);
            }
            return null;
        }

        public static void Delete(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// 默认新建的存档内容
        /// </summary>
        /// <returns></returns>
        private static SaveData BuildDefaultSaveData()
        {

            SaveData newSave = new()
            {
                myPlayerInfo = new()
                {
                    AppearanceData = new()
                    {
                        BodyId = 1, 
                        HairId = 1, 
                        ArmorId = 1,
                        SkinColor = Color.white,
                        HairColor = Color.white,
                    },
                    LV=1,
                    Name = "DefaultName",
                    Position = new Vector3(0, 0, 0),
                    MaxLife = 100,
                    Life = 100,
                    MaxPower = 100,
                    Power = 100
                },
                lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            return newSave;
        }
    }
}
