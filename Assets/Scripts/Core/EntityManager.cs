using HFantasy.Script.Common;
using HFantasy.Script.Commonpublic;
using HFantasy.Script.Configs;
using HFantasy.Script.Core.Resource;
using HFantasy.Script.Entity.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HFantasy.Script.Core
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        private PlayerEntity myPlayerEntity;
        public PlayerEntity MyPlayerEntity { get => myPlayerEntity; }

        private readonly Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();



        /// <summary>
        /// 创建一个新的玩家实体
        /// </summary>
        public PlayerEntity CreatePlayerEntity(BasicPlayerInfo playerInfo, bool isMyPlayer = false)
        {
            //string playerDataPath = EntityDataConfig.GetPlayerCareerCsvPath();
            //BasicPlayerInfo playerInfo = LoadPlayerSaveData(playerDataPath);
            //if (playerInfo == null)
            //{
            //    Debug.LogError($"加载玩家数据失败！路径：{playerDataPath}");
            //    return null;
            //}
            GameObject bodyPrefab = ConfigResManager.Instance.LoadPlayerAppearanceAsset(playerInfo.AppearanceData.BodyId);

            //GameObject playerPrefab = AssetBundleManager.Instance.LoadAssetAsync<GameObject>(bundleName,assetName);
            if (bodyPrefab == null)
            {
                Debug.LogError($"加载玩家Prefab失败！");
                return null;
            }

            int entityId = EntityIdGenerator.GetNextId();
            GameObject playerGO = new GameObject($"Player_{playerInfo.Name}_{entityId}"); //这里额外创建一个玩家的父物体
            playerGO.transform.position = playerInfo.Position;
            playerGO.transform.rotation = Quaternion.identity;
            GameObject bodyGO = Instantiate(bodyPrefab, playerGO.transform);
            
            PlayerEntity playerEntity = new PlayerEntity(entityId, playerInfo, playerGO);
            playerGO.name = $"Player_{playerInfo.Name}_{entityId}";

            playerDict.Add(entityId, playerEntity);
            if (isMyPlayer)
            {
                myPlayerEntity = playerEntity;
            }

            return playerEntity;
        }

        /// <summary>
        /// 通过ID获取玩家实体
        /// </summary>
        public PlayerEntity GetPlayerEntity(int entityId)
        {
            return playerDict.TryGetValue(entityId, out var entity) ? entity : null;
        }

    }
}