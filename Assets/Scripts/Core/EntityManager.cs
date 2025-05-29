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
             //Ԥ�������ã���ʼ�������Ϣ
            ConfigResManager.Instance.LoadPlayerAppearanceConfig();
        }

        private PlayerEntity myPlayerEntity;
        public PlayerEntity MyPlayerEntity { get => myPlayerEntity; }

        public Dictionary<int, PlayerEntity> PlayerDict => playerDict;
        private readonly Dictionary<int, PlayerEntity> playerDict = new Dictionary<int, PlayerEntity>();
        public Dictionary<int, PlayerEntity> NetIdDict => netIdDict;
        private readonly Dictionary<int, PlayerEntity> netIdDict = new Dictionary<int, PlayerEntity>();


        /// <summary>
        /// ����һ���µ����ʵ��
        /// </summary>
        public PlayerEntity CreatePlayerEntity(BasicPlayerInfo playerInfo, bool isLocalPlayer = false, string targetScene = null, int netId=-1)
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
        /// �Ƴ�һ�����ʵ��
        /// </summary>
        /// <param name="entityId">Ҫ�Ƴ���ʵ��ID</param>
        /// <returns>�Ƿ�ɹ��Ƴ�</returns>
        public bool RemovePlayerEntity(int entityId)
        {
            if (!playerDict.TryGetValue(entityId, out var entity))
            {
                Debug.LogWarning($"�����Ƴ������ڵ����ʵ�壬EntityId: {entityId}");
                return false;
            }

            // ���Ǳ�����ң����������
            if (myPlayerEntity != null && myPlayerEntity.Cid == entityId)
            {
                myPlayerEntity = null;
            }

            // ���� GameObject
            if (entity.GameObject != null)
            {
                Destroy(entity.GameObject);
            }

            playerDict.Remove(entityId);
            // Debug.Log($"�ɹ��Ƴ����ʵ�壬EntityId: {entityId}����ǰʣ��: {playerDict.Count}");
            return true;
        }
        public bool RemovePlayerEntityByNetId(int netId)
        {
            if (!netIdDict.TryGetValue(netId, out var entity))
            {
                Debug.LogWarning($"�����Ƴ������ڵ� NetId ��Ӧ���ʵ�壬NetId: {netId}");
                return false;
            }

            return RemovePlayerEntity(entity.Cid);
        }

        /// <summary>
        /// ��ȡ���е�ǰ������ҵĳ�ʼ����Ϣ������ host ת����
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
        /// �ͻ����յ�Զ����ҳ�ʼ�����ݺ󣬴�������Զ�����ʵ��
        /// </summary>
        public void CreateRemotePlayers(List<RemoteInitPlayerInfo> remotePlayers)
        {
            foreach (var remote in remotePlayers)
            {
                PlayerEntity playerEntity = CreatePlayerEntity(remote.playerInfo, isLocalPlayer: false, netId: remote.netId, targetScene:SceneConstant.ThreeDLobby);
            }
        }

        /// <summary>
        /// ͨ��ID��ȡ���ʵ��
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