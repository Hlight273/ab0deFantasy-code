using System;
using System.Collections.Generic;
using HFantasy.Script.Network.Protocol;

namespace HFantasy.Script.Network.Interface
{
    public interface IRoomService
    {
        bool IsInRoom { get; }
        RoomInfo CurrentRoom { get; }
        void Initialize();
        void CreateRoom(string roomName, int maxPlayers);
        void JoinRoom(string roomName);
        void LeaveRoom();
        void RequestRoomList();
        List<RoomInfo> GetRoomList();

        event Action<List<RoomInfo>> OnRoomListUpdated;
        event Action<RoomInfo> OnJoinedRoom;
        event Action OnLeftRoom;
    }
}