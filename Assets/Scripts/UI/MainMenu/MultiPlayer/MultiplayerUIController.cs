using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HFantasy.Script.Network;
using HFantasy.Script.Network.Config;
using HFantasy.Script.Network.Protocol;
using HFantasy.Script.Clinet;
using HFantasy.Script.Client.State;
using LiteNetLib;
using System;
using HFantasy.Script.UI.MainMenu;
using HFantasy.Script.Entity;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Core.Save;
using HFantasy.Network.Utils;
using HFantasy.Network;


namespace HFantasy.Script.UI.MultiPlayer
{
    public class MultiplayerUIController : MonoBehaviour
    {
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomItemPrefab;
        [SerializeField] private GameObject loadingPanel;

        [SerializeField] private Button refreshRoomListButton;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button joinRoomButton;

        private List<GameObject> roomItems = new List<GameObject>();
        private RoomInfo selectedRoom;

        public Action<SaveData> OnSaveSelect;

        private bool isCreatedRoom = false;


        private void OnEnable()
        {
            GameNetworkManager.Instance.JoinLobbyNetAction();
            
        }
        private void OnDisable()
        {
            GameNetworkManager.Instance.StopJoinLobbyNetAction();
            
        }


        private void Awake()
        {
            MainContext.Instance.RoomContext.RoomListChanged += RefreshRoomListUI;
            OnSaveSelect += OnSaveSelected;

            GameNetworkManager.Instance.OnClientConnected += OnRoomJoined;
            refreshRoomListButton.onClick.AddListener(OnRefreshRoomListButtonClicked);
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
        }
        private void OnDestroy()
        {
            MainContext.Instance.RoomContext.RoomListChanged -= RefreshRoomListUI;
            GameNetworkManager.Instance.OnClientConnected -= OnRoomJoined;
            refreshRoomListButton.onClick.RemoveListener(OnRefreshRoomListButtonClicked);
            createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
            joinRoomButton.onClick.RemoveListener(OnJoinRoomButtonClicked);
        }

        public void OnJoinRoomButtonClicked()
        {
            if (selectedRoom == null) return;//没有选中房间就不加入
            if (MainContext.Instance.RoomContext.RoomState != RoomConnectionState.Idle) return;//如果当前状态不是空闲就不允许加入房间
            isCreatedRoom = false;
            MainMenuUIManager.Instance.MainMenuModel.PushState(MainMenuState.MultiPlayerSaveSelect);
            loadingPanel.SetActive(true);
        }
        private void OnRoomJoined(NetPeer peer)
        {
            MainContext.Instance.RoomContext.SetRoomState(RoomConnectionState.Joining);//统一切换状态
            MainContext.Instance.CurrentPeer = peer;

            //发送加入房间请求
            var joinRequest = new JoinRoomRequest
            {
                BasicPlayerInfo = SaveSystem.CurrentSaveData.myPlayerInfo,
                RoomId = isCreatedRoom ? MainContext.Instance.RoomContext.MyRoomInfo.name : selectedRoom.name,
            };
            //新玩家加入位置都固定
            joinRequest.BasicPlayerInfo.Position = MainContext.Instance.DefaultSpawnPosition;
            var message = NetMessageHelper.Pack(NetMessageType.JoinRoomRequest, joinRequest);
            peer.Send(message, DeliveryMethod.ReliableOrdered);

        }

        private void OnRefreshRoomListButtonClicked()
        {
            //
        }


        private void OnCreateRoomButtonClicked()
        {
            if (MainContext.Instance.RoomContext.RoomState == RoomConnectionState.Joining) return;//已经创建了房间就不再创建了
            isCreatedRoom = true;
            MainMenuUIManager.Instance.MainMenuModel.PushState(MainMenuState.MultiPlayerSaveSelect);
        }
        private void OnSaveSelected(SaveData saveData)
        {

            loadingPanel.SetActive(true);
            if (isCreatedRoom)
            {
                GameNetworkManager.Instance.CreateRoomNetAction();
                MainContext.Instance.RoomContext.isHost = true;
            }
            else
            {
                GameNetworkManager.Instance.JoinRoomNetAction(selectedRoom.ip, CommonNetConfig.DefaultPort);
                MainContext.Instance.RoomContext.isHost = false;

            }
                
            SceneController.Instance.SwitchScene(SceneConstant.ThreeDLobby);
            
        }
        private void OnRoomCreated()
        {

        }

        private void RefreshRoomListUI()
        {
            var newRooms = MainContext.Instance.RoomContext.AvailableRooms;
            var newRoomDict = new Dictionary<string, RoomInfo>();
            foreach (var r in newRooms)
            {
                newRoomDict[RoomInfoKey(r)] = r;
            }

            // 当前 UI 中的项索引
            var currentRoomDict = new Dictionary<string, GameObject>();
            foreach (var itemObj in roomItems)
            {
                var ui = itemObj.GetComponent<RoomItemUI>();
                var room = ui.RoomInfo;
                currentRoomDict[RoomInfoKey(room)] = itemObj;
            }

            // Step 1: 删除 UI 中已经消失的房间
            foreach (var key in currentRoomDict.Keys)
            {
                if (!newRoomDict.ContainsKey(key))
                {
                    Destroy(currentRoomDict[key]);
                    roomItems.Remove(currentRoomDict[key]);
                }
            }

            // Step 2: 添加新房间或更新现有房间
            foreach (var kv in newRoomDict)
            {
                if (!currentRoomDict.ContainsKey(kv.Key))
                {
                    // 新房间 → 创建 UI
                    GameObject roomItemObj = Instantiate(roomItemPrefab, roomListContent);
                    var roomItemUI = roomItemObj.GetComponent<RoomItemUI>();
                    roomItemUI.SetRoomInfo(kv.Value);
                    roomItemUI.OnRoomSelected += OnRoomSelected;

                    roomItems.Add(roomItemObj);
                }
                else
                {
                    // 房间已存在 → 只更新 UI 内容（如人数变动）
                    var roomItemUI = currentRoomDict[kv.Key].GetComponent<RoomItemUI>();
                    roomItemUI.SetRoomInfo(kv.Value); // 可选：避免不必要刷新
                    roomItemUI.OnRoomSelected -= OnRoomSelected;
                    roomItemUI.OnRoomSelected += OnRoomSelected;
                }
            }

            // Step 3: 尝试恢复选中项
            if (selectedRoom != null)
            {
                string selectedKey = RoomInfoKey(selectedRoom);
                if (newRoomDict.ContainsKey(selectedKey))
                {
                    selectedRoom = newRoomDict[selectedKey];
                    HighlightSelectedRoom(selectedKey);
                }
                else
                {
                    selectedRoom = null;
                }
            }

            // Step 4: 控制 loadingPanel
            loadingPanel.SetActive(newRoomDict.Count != 0);
        }
        private void HighlightSelectedRoom(string selectedKey)
        {
            foreach (var itemObj in roomItems)
            {
                var ui = itemObj.GetComponent<RoomItemUI>();
                var info = ui.RoomInfo;
                string key = RoomInfoKey(info);

                itemObj.GetComponent<Image>().color = (key == selectedKey) ? new Color(0.7f, 0.7f, 1f) : Color.white;
            }
        }
        private void OnRoomSelected(RoomInfo room)
        {
            selectedRoom = room;

            //UI高亮并取消其他项的高亮
            foreach (var itemObj in roomItems)
            {
                var ui = itemObj.GetComponent<RoomItemUI>();
                bool isSelected = ui != null && ui == itemObj.GetComponent<RoomItemUI>();
                itemObj.GetComponent<Image>().color = isSelected ? new Color(0.7f, 0.7f, 1f) : Color.white;
            }
        }
        private string RoomInfoKey(RoomInfo room) => $"{room.ip}:{room.port}";
    }
}