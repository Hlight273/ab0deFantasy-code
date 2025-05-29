namespace HFantasy.Script.Clinet.Context
{
    using HFantasy.Script.Client.State;
    using HFantasy.Script.Common;
    using HFantasy.Script.Network.Protocol;
    using System;
    using System.Collections.Generic;
    public class RoomContext
    {
        public bool isHost { get; set; } = false;
        public RoomInfo MyRoomInfo { get; set; }
        private List<RoomInfo> _availableRooms = new();
        public IReadOnlyList<RoomInfo> AvailableRooms => _availableRooms;
        public event Action RoomListChanged;
        public void AddOrUpdateRoom(RoomInfo room)
        {
            var existing = _availableRooms.Find(r => r.ip == room.ip);
            if (existing != null)
            {
                existing.name = room.name;
                existing.curPlayerCount = room.curPlayerCount;
                existing.maxPlayerCount = room.maxPlayerCount;
            }
            else
            {
                _availableRooms.Add(room);
            }
            RoomListChanged?.Invoke();
        }
        public void ClearRooms()
        {
            _availableRooms.Clear();
            RoomListChanged?.Invoke();
        }


        public RoomConnectionState RoomState { get; private set; } = RoomConnectionState.Idle;
        public void SetRoomState(RoomConnectionState newState)
        {
            RoomState = newState;
        }
    }
}