using HFantasy.Network.Protocol;
using HFantasy.Script.Client.State;
using HFantasy.Script.Clinet.Context;
using HFantasy.Script.Common;
using HFantasy.Script.Network.Protocol;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HFantasy.Script.Clinet
{
    public class MainContext : Singleton<MainContext>
    {
        public NetPeer CurrentPeer;
        public Vector3 DefaultSpawnPosition = new Vector3(-2.21481419f, 0.287183762f, -1.36739063f);
        public List<RemoteInitPlayerInfo> RemoteInitPlayerInfos;
        public RoomContext RoomContext { get; private set; } = new RoomContext();
    }
}