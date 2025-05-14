using HFantasy.Script.Configs;
using HFantasy.Script.Entity;
using HFantasy.Script.Entity.Player;
using Mirror.Examples.MultipleMatch;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HFantasy.Script.Core.Save
{
    public class SaveSystem
    {
        //private static string saveFolder => Application.persistentDataPath + PathConfig.SaveFolder;

        public static int MaxSaveSlots => 3;
        private static int currentSaveIndex = -1;
        private static SaveData currentSaveData;
        public static SaveData CurrentSaveData { get => currentSaveData; }


        private static string saveFolder => PathConfig.SaveFolder;

        

        public static SaveData CreateNewSave(int index) => CreateNewSave(GetSaveNameFromIndex(index));
        
        private static SaveData CreateNewSave(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";

            // 防止覆盖已有存档
            if (File.Exists(path))
                return null;
            SaveData newSave = BuildDefaultSaveData();
            Save(saveFileName, newSave);
            return newSave;
        }

        public static void SaveCurrent() => Save(currentSaveIndex, Load(currentSaveIndex));
        private static void Save(int index, SaveData data) => Save(GetSaveNameFromIndex(index), data);
        private static void Save(string saveFileName, SaveData data)
        {
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFolder + saveFileName + ".json", json);
        }


        public static void SelectSaveAndEnterGame(int index, SaveData saveData=null)
        {
            if (!IsSaveIndexValid(index)) return;
            currentSaveData = saveData == null ? saveData : Load(index);
            currentSaveIndex = index;
        }
        public static void ExitAndSaveGame()
        {
            if (!IsSaveIndexValid(currentSaveIndex)) return;
            SaveCurrent();
            currentSaveData = null;
            currentSaveIndex = -1;
        }


        public static SaveData Load(int index)  => Load(GetSaveNameFromIndex(index));
        private static SaveData Load(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(json);
            }
            return null;
        }

        public static void Delete(int index) => Delete(GetSaveNameFromIndex(index));
        private static void Delete(string saveFileName)
        {
            string path = saveFolder + saveFileName + ".json";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static string GetSaveNameFromIndex(int index)
        {
            return "save" + index.ToString();
        }

        private static bool IsSaveIndexValid(int index){
            if (index < 0 || index >= MaxSaveSlots)
            {
                Debug.LogError("存档读取越界 Invalid save index: " + index);
                return false;
            }
            return true;
        }
            

        /// <summary>
        /// 默认新建的存档内容
        /// </summary>
        /// <returns></returns>
        private static SaveData BuildDefaultSaveData()
        {
            SaveData newSave =  new SaveData();
            newSave.myPlayerInfo = new BasicPlayerInfo();
            newSave.lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return newSave;
        }
    }
}
