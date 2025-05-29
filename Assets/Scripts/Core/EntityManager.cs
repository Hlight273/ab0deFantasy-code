using HFantasy.Network.Protocol;
using HFantasy.Script.Common;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Commonpublic;
using HFantasy.Script.Configs;
using HFantasy.Script.Core.Resource;
using HFantasy.Script.Entity.Player;
using HFantasy.Script.Player.Interaction;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace HFantasy.Script.Core
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        private void Start()
        {
             //预加载配置，初始化玩家信息
            ConfigResManager.Instance.LoadPlayerAppearanceConfig();
        }

        private PlayerEntity myPlayerEntity;
        public PlayerEntity MyPlayerEntity { get => myPlayerEntity; }

        public Dictionary<int, PlayerEntity> PlayerDict => playerDict;
        private readonly Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();
        public Dictionary<int, PlayerEntity> NetIdDict => netIdDict;
        private readonly Dictionary<int, PlayerEntity> netIdDict = new Dictionary<int, PlayerEntity>();


        /// <summary>
        /// 创建一个新的玩家实体
        /// </summary>
        public PlayerEntity CreatePlayerEntity(BasicPlayerInfo playerInfo, bool isLocalPlayer = false, string targetScene = null, int netId=-1)
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
            if (targetScene != null) { 
                SceneManager.MoveGameObjectToScene(playerGO, SceneManager.GetSceneByName(targetScene));//最好指定targetScene，防止后续逻辑出错
            }


            playerGO.layer = LayerConstant.Player;
            playerGO.transform.position = playerInfo.Position;
            playerGO.transform.rotation = Quaternion.identity;
            GameObject bodyGO = Instantiate(bodyPrefab, playerGO.transform);
            
            PlayerEntity playerEntity = new PlayerEntity(entityId, playerInfo, playerGO, isLocalPlayer, netId);
            playerGO.name = $"Player_{playerInfo.Name}_{entityId}";

            PlayerDict.Add(entityId, playerEntity);
            if (netId >= 0)
            {
                netIdDict[netId] = playerEntity;
            }
            if (isLocalPlayer)
            {
                myPlayerEntity = playerEntity;
            }
            //Debug.Log($"PlayerDict Count after Add: {PlayerDict.Count}");
            return playerEntity;
        }

        /// <summary>
        /// 移除一个玩家实体
        /// </summary>
        /// <param name="entityId">要移除的实体ID</param>
        /// <returns>是否成功移除</returns>
        public bool RemovePlayerEntity(int entityId)
        {
            if (!playerDict.TryGetValue(entityId, out var entity))
            {
                Debug.LogWarning($"尝试移除不存在的玩家实体，EntityId: {entityId}");
                return false;
            }

            // 若是本地玩家，则清空引用
            if (myPlayerEntity != null && myPlayerEntity.Cid == entityId)
            {
                myPlayerEntity = null;
            }

            // 销毁 GameObject
            if (entity.GameObject != null)
            {
                Destroy(entity.GameObject);
            }

            playerDict.Remove(entityId);
            // Debug.Log($"成功移除玩家实体，EntityId: {entityId}，当前剩余: {playerDict.Count}");
            return true;
        }
        public bool RemovePlayerEntityByNetId(int netId)
        {
            if (!netIdDict.TryGetValue(netId, out var entity))
            {
                Debug.LogWarning($"尝试移除不存在的 NetId 对应玩家实体，NetId: {netId}");
                return false;
            }

            return RemovePlayerEntity(entity.Cid);
        }

        /// <summary>
        /// 获取所有当前在线玩家的初始化信息（用于 host 转发）
        /// </summary>
        public List<RemoteInitPlayerInfo> GetRemoteInitPlayerInfos()
        {
            var list = new List<RemoteInitPlayerInfo>();

            foreach (var player in playerDict.Values)
            {
                if (player.Info == null || player.PlayerObject == null)
                    continue;

                var initInfo = new RemoteInitPlayerInfo(player.Info, player.PlayerObject.transform.position, player.NetId);
                list.Add(initInfo);
            }

            return list;
        }
        /// <summary>
        /// 客户端收到远端玩家初始化数据后，创建所有远端玩家实体
        /// </summary>
        public void CreateRemotePlayers(List<RemoteInitPlayerInfo> remotePlayers)
        {
            foreach (var remote in remotePlayers)
            {
                PlayerEntity playerEntity = CreatePlayerEntity(remote.playerInfo, isLocalPlayer: false, netId: remote.netId, targetScene:SceneConstant.ThreeDLobby);
            }
        }

        /// <summary>
        /// 通过ID获取玩家实体
        /// </summary>
        public PlayerEntity GetPlayerEntity(int entityId)
        {
            return PlayerDict.TryGetValue(entityId, out var entity) ? entity : null;
        }
        public PlayerEntity GetPlayerEntityByNetId(int netId)
        {
            return netIdDict.TryGetValue(netId, out var entity) ? entity : null;
        }

        private void Update()
        {
            //Debug.Log($"PlayerDict Count in Update: {PlayerDict.Count}");
            foreach (var player in PlayerDict.Values)
            {
                player.Update();
            }
        }

    }
}