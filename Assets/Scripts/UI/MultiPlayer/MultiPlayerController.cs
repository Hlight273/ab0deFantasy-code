using HFantasy.Script.Network.Core;
using HFantasy.Script.Network.Room;
using HFantasy.Script.Network.Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace HFantasy.Script.UI.MultiPlayer
{
    public class MultiPlayerController : MonoBehaviour
    {
        [Header("房间列表")]
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomItemPrefab;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button joinRoomButton;
        [SerializeField] private Button refreshButton;

        private float refreshTimer = 0f;
        private const float REFRESH_INTERVAL = 5f;
        private RoomInfo selectedRoom;

        private void Start()
        {
           

           
        }

        private void OnEnable()
        {
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            joinRoomButton.onClick.AddListener(OnJoinRoomButtonClicked);
            refreshButton.onClick.AddListener(RefreshRoomList);

            joinRoomButton.interactable = false;

            RoomManager.Instance.Initialize();
            RoomManager.Instance.OnRoomListUpdated += OnRoomListUpdated;
            RoomManager.Instance.OnJoinedRoom += OnJoinedRoom;
        }
        private void OnDisable()
        {
            StopAllCoroutines();
            createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
            joinRoomButton.onClick.RemoveListener(OnJoinRoomButtonClicked);
            refreshButton.onClick.RemoveListener(RefreshRoomList);
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomListUpdated -= OnRoomListUpdated;
                RoomManager.Instance.OnJoinedRoom -= OnJoinedRoom;
            }
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }
        }
        

        private void OnDestroy()
        {
           
        }

        private void Update()
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer >= REFRESH_INTERVAL)
            {
                refreshTimer = 0f;
                RefreshRoomList();
            }
        }

        private void OnCreateRoomButtonClicked()
        {
            createRoomButton.interactable = false;
            NetworkManager.Instance.StartHost(7777);
            string roomName = "Room" + UnityEngine.Random.Range(1000, 9999);
            RoomManager.Instance.CreateRoom(roomName, 4);
        }

        private void OnJoinRoomButtonClicked()
        {
            if (selectedRoom != null)
            {
                // 加入房间时作为客户端连接到房主
                NetworkManager.Instance.StartClient(selectedRoom.HostAddress, 7777);
                RoomManager.Instance.JoinRoom(selectedRoom.Name);
            }
        }

        private void RefreshRoomList()
        {
            RoomManager.Instance.RequestRoomList();
        }

        private void OnRoomListUpdated(List<RoomInfo> rooms)
        {
            UpdateRoomList(rooms);
        }

        private void OnJoinedRoom(RoomInfo room)
        {
            // 处理加入房间后的UI逻辑
        }

        private void UpdateRoomList(List<RoomInfo> rooms)
        {
            // 清理现有列表
    foreach (Transform child in roomListContent)
    {
        Destroy(child.gameObject);
    }

    // 创建新的房间项
    foreach (var room in rooms)
    {
        GameObject roomItem = Instantiate(roomItemPrefab, roomListContent);
        RoomItemUI roomItemUI = roomItem.GetComponent<RoomItemUI>();
        roomItemUI.SetRoomInfo(room);
        roomItemUI.OnRoomSelected += OnRoomSelected;
    }
        }

        private void OnRoomSelected(RoomInfo room)
{
    selectedRoom = room;
    joinRoomButton.interactable = true;
}
    }
    
}