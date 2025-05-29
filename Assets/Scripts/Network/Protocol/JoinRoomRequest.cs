using HFantasy.Network.Protocol;
using HFantasy.Script.Entity;
using HFantasy.Script.Entity.Player;
using System;
using System.Collections.Generic;

namespace HFantasy.Script.Network.Protocol
{

    [Serializable]
    public class JoinRoomRequest
    {
        public BasicPlayerInfo BasicPlayerInfo;
        public string RoomId;
    }

    [Serializable]
    public class JoinRoomResponse
    {
        public bool Success;
        public string Message;
        public List<RemoteInitPlayerInfo> RemoteInitPlayerInfos;
    }
}