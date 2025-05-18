using HFantasy.Script.Network.Interface;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace HFantasy.Script.Network.Protocol
{
    public enum MessageType : byte
    {
        RoomInfo = 1,
        CreateRoom,
        JoinRoom,
        LeaveRoom,
        RoomList,
        JoinRoomResult,
        LeaveRoomResult,
        RequestRoomList
    }

    public class RoomInfo : INetworkMessage
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public List<string> Players { get; set; } = new List<string>();
        public string HostAddress { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.RoomInfo;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Name);
            writer.Put(MaxPlayers);
            writer.Put(CurrentPlayers);
            writer.Put(HostAddress);
            writer.Put(Players.Count);
            foreach (var player in Players)
            {
                writer.Put(player);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            Name = reader.GetString();
            MaxPlayers = reader.GetInt();
            CurrentPlayers = reader.GetInt();
            HostAddress = reader.GetString();
            int count = reader.GetInt();
            Players.Clear();
            for (int i = 0; i < count; i++)
            {
                Players.Add(reader.GetString());
            }
        }
    }

    public class CreateRoomMessage : INetworkMessage
    {
        public string RoomName { get; set; }
        public int MaxPlayers { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.CreateRoom;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(RoomName);
            writer.Put(MaxPlayers);
        }

        public void Deserialize(NetDataReader reader)
        {
            RoomName = reader.GetString();
            MaxPlayers = reader.GetInt();
        }
    }

    public class JoinRoomMessage : INetworkMessage
    {
        public string RoomName { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.JoinRoom;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(RoomName);
        }

        public void Deserialize(NetDataReader reader)
        {
            RoomName = reader.GetString();
        }
    }

    public class LeaveRoomMessage : INetworkMessage
    {
        public string RoomName { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.LeaveRoom;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(RoomName);
        }

        public void Deserialize(NetDataReader reader)
        {
            RoomName = reader.GetString();
        }
    }

    public class RoomListMessage : INetworkMessage
    {
        public List<RoomInfo> Rooms { get; set; } = new List<RoomInfo>();

        public byte MessageType => (byte)Protocol.MessageType.RoomList;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Rooms.Count);
            foreach (var room in Rooms)
            {
                room.Serialize(writer);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            int count = reader.GetInt();
            Rooms.Clear();
            for (int i = 0; i < count; i++)
            {
                var room = new RoomInfo();
                room.Deserialize(reader);
                Rooms.Add(room);
            }
        }
    }

    public class JoinRoomResultMessage : INetworkMessage
    {
        public bool Success { get; set; }
        public RoomInfo Room { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.JoinRoomResult;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Success);
            if (Success && Room != null)
            {
                writer.Put(true);
                Room.Serialize(writer);
            }
            else
            {
                writer.Put(false);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            Success = reader.GetBool();
            if (Success && reader.GetBool())
            {
                Room = new RoomInfo();
                Room.Deserialize(reader);
            }
        }
    }

    public class LeaveRoomResultMessage : INetworkMessage
    {
        public bool Success { get; set; }

        public byte MessageType => (byte)Protocol.MessageType.LeaveRoomResult;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Success);
        }

        public void Deserialize(NetDataReader reader)
        {
            Success = reader.GetBool();
        }
    }

    public class RequestRoomListMessage : INetworkMessage
    {
        public byte MessageType => (byte)Protocol.MessageType.RequestRoomList;

        public void Serialize(NetDataWriter writer) { }
        public void Deserialize(NetDataReader reader) { }
    }
}