using HFantasy.Script.Common;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Commonpublic;
using HFantasy.Script.Configs;
using HFantasy.Script.Core.Resource;
using HFantasy.Script.Entity.Player;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HFantasy.Script.Core
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        private PlayerEntity myPlayerEntity;
        public PlayerEntity MyPlayerEntity { get => myPlayerEntity; }

        public Dictionary<int, PlayerEntity> PlayerDict => playerDict;


        private readonly Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();



        /// <summary>
        /// ����һ���µ����ʵ��
        /// </summary>
        public PlayerEntity CreatePlayerEntity(BasicPlayerInfo playerInfo, bool isLocalPlayer = false, string targetScene = null)
        {
            //string playerDataPath = EntityDataConfig.GetPlayerCareerCsvPath();
            //BasicPlayerInfo playerInfo = LoadPlayerSaveData(playerDataPath);
            //if (playerInfo == null)
            //{
            //    Debug.LogError($"�����������ʧ�ܣ�·����{playerDataPath}");
            //    return null;
            //}
            GameObject bodyPrefab = ConfigResManager.Instance.LoadPlayerAppearanceAsset(playerInfo.AppearanceData.BodyId);

            //GameObject playerPrefab = AssetBundleManager.Instance.LoadAssetAsync<GameObject>(bundleName,assetName);
            if (bodyPrefab == null)
            {
                Debug.LogError($"�������Prefabʧ�ܣ�");
                return null;
            }

            int entityId = EntityIdGenerator.GetNextId();
            GameObject playerGO = new GameObject($"Player_{playerInfo.Name}_{entityId}"); //������ⴴ��һ����ҵĸ�����
            if (targetScene != null) { 
                SceneManager.MoveGameObjectToScene(playerGO, SceneManager.GetSceneByName(targetScene));//���ָ��targetScene����ֹ�����߼�����
            }


            playerGO.layer = LayerConstant.Player;
            playerGO.transform.position = playerInfo.Position;
            playerGO.transform.rotation = Quaternion.identity;
            GameObject bodyGO = Instantiate(bodyPrefab, playerGO.transform);
            
            PlayerEntity playerEntity = new PlayerEntity(entityId, playerInfo, playerGO, isLocalPlayer);
            playerGO.name = $"Player_{playerInfo.Name}_{entityId}";

            PlayerDict.Add(entityId, playerEntity);
            if (isLocalPlayer)
            {
                myPlayerEntity = playerEntity;
            }

            return playerEntity;
        }

        /// <summary>
        /// ͨ��ID��ȡ���ʵ��
        /// </summary>
        public PlayerEntity GetPlayerEntity(int entityId)
        {
            return PlayerDict.TryGetValue(entityId, out var entity) ? entity : null;
        }

        private void Update()
        {
            foreach (var player in PlayerDict.Values)
            {
                player.Update();
            }
        }

    }
}