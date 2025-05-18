using HFantasy.Script.Network.Interface;
using HFantasy.Script.Network.Protocol;
using HFantasy.Script.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using HFantasy.Script.Network.Core;
using System.Net;

namespace HFantasy.Script.Network.Room
{
    public class RoomManager : Singleton<RoomManager>, IRoomService, IDisposable
    {
        private RoomInfo currentRoom;
        private List<RoomInfo> roomList = new List<RoomInfo>();
        private bool initialized;

        public bool IsInRoom => currentRoom != null;
        public RoomInfo CurrentRoom => currentRoom;

        public event Action<List<RoomInfo>> OnRoomListUpdated;
        public event Action<RoomInfo> OnJoinedRoom;
        public event Action OnLeftRoom;

        public void Initialize()
        {
            if (initialized) return;
            RegisterNetworkHandlers();
            initialized = true;

            // ��ʼ��ʱ���󷿼��б�
            RequestRoomList();
        }

        private void RegisterNetworkHandlers()
        {
            var networkManager = NetworkManager.Instance;
            networkManager.RegisterHandler<RoomListMessage>(OnRoomListReceived);
            networkManager.RegisterHandler<JoinRoomResultMessage>(OnJoinRoomResultReceived);
            networkManager.RegisterHandler<LeaveRoomResultMessage>(OnLeaveRoomResultReceived);
            networkManager.RegisterHandler<CreateRoomMessage>(OnCreateRoomReceived); // ��Ӵ�������Ĵ���
        }

        private void OnCreateRoomReceived(CreateRoomMessage message)
        {
            //�����·���
            var room = new RoomInfo
            {
                Name = message.RoomName,
                MaxPlayers = message.MaxPlayers,
                CurrentPlayers = 1,
                HostAddress = GetLocalIPAddress()
            };

            roomList.Add(room);
            currentRoom = room;

            //֪ͨUI����
            OnRoomListUpdated?.Invoke(roomList);
            OnJoinedRoom?.Invoke(room);
        }

        public void CreateRoom(string roomName, int maxPlayers)
{
    if (!initialized)
    {
        Debug.LogError("RoomManager not initialized!");
        return;
    }

    // ֱ�Ӵ������䣬��Ϊ�����Ѿ�������
    var room = new RoomInfo
    {
        Name = roomName,
        MaxPlayers = maxPlayers,
        CurrentPlayers = 1,
        HostAddress = GetLocalIPAddress()
    };

    roomList.Add(room);
    currentRoom = room;
    OnRoomListUpdated?.Invoke(roomList);
    OnJoinedRoom?.Invoke(room);

    // �㲥���䴴����Ϣ
    var message = new CreateRoomMessage
    {
        RoomName = roomName,
        MaxPlayers = maxPlayers
    };
    NetworkManager.Instance.SendMessage(message);
}

        public void JoinRoom(string roomName)
        {
            if (!initialized)
            {
                Debug.LogError("RoomManager not initialized!");
                return;
            }

            var message = new JoinRoomMessage { RoomName = roomName };
            NetworkManager.Instance.SendMessage(message);
        }

        public void LeaveRoom()
        {
            if (!initialized)
            {
                Debug.LogError("RoomManager not initialized!");
                return;
            }

            if (currentRoom != null)
            {
                var message = new LeaveRoomMessage { RoomName = currentRoom.Name };
                NetworkManager.Instance.SendMessage(message);
            }
        }

        public void RequestRoomList()
        {
            if (!initialized)
            {
                Debug.LogError("RoomManager not initialized!");
                return;
            }

            var message = new RequestRoomListMessage();
            NetworkManager.Instance.SendMessage(message);
        }

        public List<RoomInfo> GetRoomList()
        {
            return new List<RoomInfo>(roomList);
        }

        private void OnRoomListReceived(RoomListMessage message)
        {
            roomList = message.Rooms;
            OnRoomListUpdated?.Invoke(roomList);
        }

        private void OnJoinRoomResultReceived(JoinRoomResultMessage message)
        {
            if (message.Success)
            {
                currentRoom = message.Room;
                OnJoinedRoom?.Invoke(currentRoom);
            }
        }

        private void OnLeaveRoomResultReceived(LeaveRoomResultMessage message)
        {
            if (message.Success)
            {
                currentRoom = null;
                OnLeftRoom?.Invoke();
            }
        }

        public void Dispose()
        {
            var networkManager = NetworkManager.Instance;
            networkManager.UnregisterHandler<RoomListMessage>(OnRoomListReceived);
            networkManager.UnregisterHandler<JoinRoomResultMessage>(OnJoinRoomResultReceived);
            networkManager.UnregisterHandler<LeaveRoomResultMessage>(OnLeaveRoomResultReceived);
            initialized = false;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1"; // ����Ҳ��������ر��ػػ���ַ
        }

    }
}