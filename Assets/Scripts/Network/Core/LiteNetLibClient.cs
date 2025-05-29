using HFantasy.Network.Utils;
using HFantasy.Network;
using HFantasy.Script.Network.Protocol;
using LiteNetLib;
using System;
using UnityEngine;
using HFantasy.Script.Core;
using HFantasy.Script.Entity.Player;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Clinet;

public class LiteNetLibClient
{
    private NetManager _client;
    private EventBasedNetListener _listener;

    private NetPeer _connectedPeer;
    public NetPeer ConnectedPeer => _connectedPeer;

    public event Action<NetPeer> OnConnected;
    public event Action<NetPeer, DisconnectInfo> OnDiscnnected;

    public void StartClient(string ip, int port)
    {
        _listener = new EventBasedNetListener();
        _client = new NetManager(_listener);
        bool success = _client.Start();
        Debug.Log("客户端启动成功：" + success);
        NetPeer netPeer = _client.Connect(ip, port, "room_key");
        Debug.Log("客户端连接成功：" + netPeer.Address+":"+netPeer.Port);

        _listener.PeerConnectedEvent += peer =>
        {
            _connectedPeer = peer;
            Debug.Log($"客户端连接到服务器: {peer.Address}:{peer.Port}");
            OnConnected?.Invoke(peer);
        };

        _listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            byte[] raw = new byte[reader.AvailableBytes];
            reader.GetBytes(raw, 0, raw.Length);
            reader.Recycle();

            var msg = NetMessageHelper.Unpack(raw);
            switch (msg.MsgType)
            {
                case NetMessageType.JoinRoomResponse:
                    var res = NetMessageHelper.DecodePayload<JoinRoomResponse>(msg);
                    Debug.Log($"加入房间结果: {(res.Success ? "成功" : "失败")} - {res.Message}");
                    if (res.Success)
                    {
                        //加入房间成功，但是不能直接创建本地玩家实体，要在场景加载完成后再创建
                        //EntityManager.Instance.CreateRemotePlayers(res.RemoteInitPlayerInfos);
                        MainContext.Instance.RemoteInitPlayerInfos = res.RemoteInitPlayerInfos;
                        //通知服务器玩家加入成功
                        //var joinBroadcast = new JoinRoomRequest { BasicPlayerInfo = res.BasicPlayerInfo };
                        //_client.SendToAll(NetMessageHelper.Pack(NetMessageType.PlayerJoinedBroadcast, joinBroadcast));
                    }
                    else
                    {
                        //加入失败
                        Debug.LogError("加入房间失败: " + res.Message);
                    }
                    break;
                case NetMessageType.PlayerJoinedBroadcast:
                    {
                        var data = NetMessageHelper.DecodePayload<JoinRoomRequest>(msg);
                        Debug.Log($"收到玩家加入广播：{data.BasicPlayerInfo.Name}");

                        //创建远程玩家
                        PlayerEntity playerEntity = EntityManager.Instance.CreatePlayerEntity(data.BasicPlayerInfo, false, SceneConstant.ThreeDLobby, peer.Id);
                        break;
                    }
                case NetMessageType.PlayeExitBroadcast:
                    {
                        var data = NetMessageHelper.DecodePayload<NetPlayerInfo>(msg);
                        Debug.Log($"收到玩家离开广播：{data.Name}");
                        //移除远程玩家
                        EntityManager.Instance.RemovePlayerEntityByNetId(data.Peer.Id);
                        break;
                    }
            }
        };

        _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            Debug.LogWarning($"客户端与服务器断开连接: {peer.Address}:{peer.Port}, 原因: {disconnectInfo.Reason}");
            _connectedPeer = null;
            OnDiscnnected?.Invoke(peer, disconnectInfo);
        };
        _listener.NetworkErrorEvent += (endPoint, error) =>
        {
            Debug.LogError($"客户端连接服务器网络错误 {endPoint}: {error}");
        };
    }


    public void PollEvents()
    {
        _client.PollEvents();
    }

    public void Stop()
    {
        _client.Stop();
    }
}
