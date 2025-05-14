using HFantasy.Script.Common;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Configs;
using HFantasy.Script.Core.Resource;
using HFantasy.Script.Core.Save;
using HFantasy.Script.Entity;
using HFantasy.Script.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HFantasy.Script.Core
{
    [NonPersistentSingleton]
    public class GameController : MonoSingleton<GameController>
    {
        void Start()
        {
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
#if UNITY_ANDROID
            //清理持久化目录中的 AB 包
            string persistentABPath = Path.Combine(Application.persistentDataPath, "AssetBundle");
            if (Directory.Exists(persistentABPath))
            {
                try
                {
                    Directory.Delete(persistentABPath, true);
                    Debug.Log("已清理 AB 包缓存目录");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"清理 AB 包缓存失败: {e.Message}");
                }
            }
            //等待AB初始化完成
            bool initialized = ABInitializer.IsInitialized;
            if (!initialized)
            {
                System.Action onInitialized = () => initialized = true;
                ABInitializer.OnABInitialized += onInitialized;
                Debug.Log("xx:" + ABInitializer.IsInitialized);
                yield return new WaitUntil(() => initialized);
                ABInitializer.OnABInitialized -= onInitialized;
            }
            Debug.Log("xxx:" + ABInitializer.IsInitialized);
#endif

            //初始化玩家信息
            ConfigResManager.Instance.LoadPlayerAppearanceConfig();

            //SaveSystem.CreateNewSave(0);
            SaveData saveData = SaveSystem.CurrentSaveData;
            Transform spawnPoint = GameObject.Find("Room").transform.Find("SpawnPoint");
            Transform dummyPoint = GameObject.Find("Room").transform.Find("DummyPlayerPoint");
            Transform dummyPoint2 = GameObject.Find("Room").transform.Find("DummyPlayerPoint2");
            saveData.myPlayerInfo.Position = spawnPoint.position;

            PlayerEntity myPlayer = EntityManager.Instance.CreatePlayerEntity(saveData.myPlayerInfo, true, SceneConstant.ThreeDLobby);
            PlayerEntity testDummyPlayer = EntityManager.Instance.CreatePlayerEntity(PlayerInfoTemplate.BuildDefaultPlayerInfo(dummyPoint.position),false, SceneConstant.ThreeDLobby);
            PlayerEntity testDummyPlayer2 = EntityManager.Instance.CreatePlayerEntity(PlayerInfoTemplate.BuildDefaultPlayerInfo(dummyPoint2.position), false, SceneConstant.ThreeDLobby);
            testDummyPlayer2.Info.Life = 99999;

            MainCameraController mc = Camera.main.GetComponent<MainCameraController>();
            mc.target = myPlayer.PlayerObject.transform;
            yield return null;
        }

        void OnDestroy()
        {

        }
    }
}
