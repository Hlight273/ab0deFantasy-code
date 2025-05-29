using HFantasy.Script.Entity.Player;
using System;
using UnityEngine;


namespace HFantasy.Network.Protocol
{

    [Serializable]
    public class RemoteInitPlayerInfo
    {
        public BasicPlayerInfo playerInfo;
        public Vector3 position;
        public int netId;
        public RemoteInitPlayerInfo(BasicPlayerInfo playerInfo, Vector3 position, int netId )
        {
            this.playerInfo = playerInfo;
            this.position = position;
            this.netId = netId;
        }
    }
}