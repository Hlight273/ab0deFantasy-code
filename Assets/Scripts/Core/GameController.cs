using HFantasy.Script.Common;
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
    public class GameController : MonoSingleton<GameController>
    {


        void Start()
        {
            StartCoroutine(InitGame());
        }

        private IEnumerator InitGame()
        {
            //����־û�Ŀ¼�е� AB ��
            string persistentABPath = Path.Combine(Application.persistentDataPath, "AssetBundle");
            if (Directory.Exists(persistentABPath))
            {
                try
                {
                    Directory.Delete(persistentABPath, true);
                    Debug.Log("������ AB ������Ŀ¼");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"���� AB ������ʧ��: {e.Message}");
                }
            }
            //�ȴ�AB��ʼ�����
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

            //��ʼ�������Ϣ
            ConfigResManager.Instance.LoadPlayerAppearanceConfig();

            SaveSystem.CreateNewSave("save1");
            SaveData saveData = SaveSystem.Load("save1");
            Transform spawnPoint = GameObject.Find("Room").transform.Find("SpawnPoint");
            Transform dummyPoint = GameObject.Find("Room").transform.Find("DummyPlayerPoint");
            Transform dummyPoint2 = GameObject.Find("Room").transform.Find("DummyPlayerPoint2");
            saveData.myPlayerInfo.Position = spawnPoint.position;

            PlayerEntity myPlayer = EntityManager.Instance.CreatePlayerEntity(saveData.myPlayerInfo, true);
            PlayerEntity testDummyPlayer = EntityManager.Instance.CreatePlayerEntity(PlayerInfoTemplate.BuildDefaultPlayerInfo(dummyPoint.position));
            PlayerEntity testDummyPlayer2 = EntityManager.Instance.CreatePlayerEntity(PlayerInfoTemplate.BuildDefaultPlayerInfo(dummyPoint2.position));
            testDummyPlayer2.Info.Life = 99999;

            Camera.main.GetComponent<MainCameraController>().target = myPlayer.PlayerObject.transform;
        }

        void OnDestroy()
        {

        }
    }
}
