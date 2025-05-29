using System;

namespace HFantasy.Network
{
    [Serializable]
    public class NetMessage
    {
        public NetMessageType MsgType;
        public byte[] Payload;
    }

    public enum NetMessageType
    {
        JoinRoomRequest = 1,
        JoinRoomResponse = 2,
        PlayerJoinedBroadcast = 3,
        PlayeExitBroadcast = 4,
    }

}