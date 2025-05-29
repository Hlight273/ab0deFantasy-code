
//using HFantasy.Script.Network.Core;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using HFantasy.Script.Network.Protocol;

//namespace HFantasy.Script.UI.MultiPlayer
//{
//    public class PlayerListController : MonoBehaviour
//    {
//        [SerializeField] private Transform playerListContent;
//        [SerializeField] private GameObject playerItemPrefab;
//        [SerializeField] private TextMeshProUGUI roomNameText;

//        private void OnEnable()
//        {
//            if (NetworkManager.Instance.IsHost)
//            {
//                // 主机模式
//                if (ServerRoomManager.Instance.CurrentRoom != null)
//                {
//                    UpdateRoomInfo(ServerRoomManager.Instance.CurrentRoom);
//                }
//            }
//            else
//            {
//                // 客户端模式
//                ClientRoomManager.Instance.OnPlayerJoined += OnPlayerJoined;
//                ClientRoomManager.Instance.OnPlayerLeft += OnPlayerLeft;
                
//                if (ClientRoomManager.Instance.CurrentRoom != null)
//                {
//                    UpdateRoomInfo(ClientRoomManager.Instance.CurrentRoom);
//                    UpdatePlayerList(ClientRoomManager.Instance.PlayersInRoom);
//                }
//            }
//        }

//        private void OnDisable()
//        {
//            if (!NetworkManager.Instance.IsHost && ClientRoomManager.Instance != null)
//            {
//                ClientRoomManager.Instance.OnPlayerJoined -= OnPlayerJoined;
//                ClientRoomManager.Instance.OnPlayerLeft -= OnPlayerLeft;
//            }
            
//            ClearPlayerList();
//        }

//        private void UpdateRoomInfo(RoomInfo roomInfo)
//        {
//            if (roomInfo != null)
//            {
//                roomNameText.text = $"房间: {roomInfo.Name} ({roomInfo.CurrentPlayers}/{roomInfo.MaxPlayers})";
//            }
//        }

//        private void OnPlayerJoined(string playerName)
//        {
//            // 更新房间信息
//            if (ClientRoomManager.Instance.CurrentRoom != null)
//            {
//                UpdateRoomInfo(ClientRoomManager.Instance.CurrentRoom);
//            }
            
//            // 添加新玩家到列表
//            AddPlayerToList(playerName);
//        }

//        private void OnPlayerLeft(string playerName)
//        {
//            // 更新房间信息
//            if (ClientRoomManager.Instance.CurrentRoom != null)
//            {
//                UpdateRoomInfo(ClientRoomManager.Instance.CurrentRoom);
//            }
            
//            // 从列表中移除玩家
//            RemovePlayerFromList(playerName);
//        }

//        private void UpdatePlayerList(List<string> players)
//        {
//            ClearPlayerList();
            
//            foreach (var playerName in players)
//            {
//                AddPlayerToList(playerName);
//            }
//        }

//        private void AddPlayerToList(string playerName)
//        {
//            GameObject playerItem = Instantiate(playerItemPrefab, playerListContent);
//            playerItem.GetComponentInChildren<TextMeshProUGUI>().text = playerName;
//            playerItem.name = $"Player_{playerName}";
//        }

//        private void RemovePlayerFromList(string playerName)
//        {
//            Transform playerItem = playerListContent.Find($"Player_{playerName}");
//            if (playerItem != null)
//            {
//                Destroy(playerItem.gameObject);
//            }
//        }

//        private void ClearPlayerList()
//        {
//            foreach (Transform child in playerListContent)
//            {
//                Destroy(child.gameObject);
//            }
//        }
//    }
//}