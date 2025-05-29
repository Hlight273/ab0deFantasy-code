using HFantasy.Network.Utils;
using HFantasy.Network;
using HFantasy.Script.Entity.Player;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;
using HFantasy.Script.Network.Protocol;

public class RoomManager
{
    private static RoomManager _instance;
    public static RoomManager Instance => _instance ??= new RoomManager();

    private Dictionary<int, NetPlayerInfo> _players = new();

    public void AddPlayer(NetPlayerInfo player)
    {
        if (_players.ContainsKey(player.Peer.Id)) return;

        _players[player.Peer.Id] = player;
        Debug.Log($"RoomManager: 玩家 {player.Name} 加入房间");
    }

    public void RemovePlayer(NetPeer peer)
    {
        if (_players.Remove(peer.Id, out var info))
        {
            Debug.Log($"RoomManager: 玩家 {info.Name} 离开房间");
        }
    }

    public IReadOnlyCollection<NetPlayerInfo> GetAllPlayers() => _players.Values;

    public void Broadcast(byte[] data, DeliveryMethod method = DeliveryMethod.ReliableOrdered, NetPeer exclude = null)
    {
        foreach (var player in _players.Values)
        {
            if (player.Peer != exclude)
            {
                player.Peer.Send(data, method);
            }
        }
    }

    public void Clear()
    {
        _players.Clear();
    }

    public void BroadcastPlayerJoined(NetPlayerInfo newPlayer)
    {
        var joinNotify = new JoinRoomRequest
        {
            BasicPlayerInfo = newPlayer.BasicInfo,
            RoomId = "default_room"
        };

        var data = NetMessageHelper.Pack(NetMessageType.PlayerJoinedBroadcast, joinNotify);

        foreach (var player in _players.Values)
        {
            //避免给自己
            if (player.Peer.Id != newPlayer.Peer.Id)
            {
                player.Peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }
    }
    public void BroadcastPlayerExit(NetPeer peer)
    {
        NetPlayerInfo playerInfo=null;
        _players.TryGetValue(peer.Id, out playerInfo);
        if (playerInfo == null)
        {
            Debug.LogWarning($"RoomManager: 玩家 {peer.Id} 不在房间中，无法发送离开通知");
            return;
        }
        var data = NetMessageHelper.Pack(NetMessageType.PlayeExitBroadcast, playerInfo);

        foreach (var player in _players.Values)
        {
            if (player.Peer.Id != playerInfo.Peer.Id)
            {
                player.Peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }
    }
}
