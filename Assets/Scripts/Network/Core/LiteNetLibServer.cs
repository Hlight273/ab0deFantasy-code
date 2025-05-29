using HFantasy.Network.Utils;
using HFantasy.Network;
using HFantasy.Script.Network.Protocol;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HFantasy.Script.Core;

public class LiteNetLibServer
{
    private NetManager _server;
    private EventBasedNetListener _listener;

    public event Action<NetPeer> OnConnected;
    public event Action<NetPeer, DisconnectInfo> OnDisconnected;

    //RoomManager存房间所有信息
    public IReadOnlyCollection<NetPlayerInfo> GetConnectedPlayers() => RoomManager.Instance.GetAllPlayers();

    public int GetCurrentPlayerCount() => _server?.ConnectedPeersCount ?? -1;

    public void StartServer(int port)
    {
        _listener = new EventBasedNetListener();
        _server = new NetManager(_listener);
        bool started = _server.Start(port);
        if (!started)
        {
            Debug.LogError("服务器启动失败！");
        }

        _listener.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey("room_key");
        };

        _listener.PeerConnectedEvent += peer =>
        {
            Debug.Log($"host：客户端 {peer.Address}:{peer.Port} 尝试连接到了 本机端口 {_server.LocalPort}，等待发送玩家信息");
            OnConnected?.Invoke(peer);
        };

        _listener.NetworkReceiveEvent += (peer, reader, channel, method) =>
        {
            byte[] raw = new byte[reader.AvailableBytes];
            reader.GetBytes(raw, 0, raw.Length);
            reader.Recycle();

            var msg = NetMessageHelper.Unpack(raw);
            Debug.Log(NetMessageHelper.DebugDump(msg));

            switch (msg.MsgType)
            {
                case NetMessageType.JoinRoomRequest:
                    {
                        var joinReq = NetMessageHelper.DecodePayload<JoinRoomRequest>(msg);
                        Debug.Log($"玩家 {joinReq.BasicPlayerInfo.Name} 请求加入 {joinReq.RoomId}");

                        var netPlayer = new NetPlayerInfo(peer, joinReq.BasicPlayerInfo);
                        RoomManager.Instance.AddPlayer(netPlayer);

                        //回复客户端加入成功
                        var response = new JoinRoomResponse
                        {
                            Success = true,
                            Message = "加入成功",
                            RemoteInitPlayerInfos = EntityManager.Instance.GetRemoteInitPlayerInfos()
                        };

                        var responseData = NetMessageHelper.Pack(NetMessageType.JoinRoomResponse, response);
                        peer.Send(responseData, DeliveryMethod.ReliableOrdered);

                        //广播玩家加入
                        RoomManager.Instance.BroadcastPlayerJoined(netPlayer);
                        break;
                    }
            }
        };

        _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            RoomManager.Instance.BroadcastPlayerExit(peer);
            RoomManager.Instance.RemovePlayer(peer);
            Debug.Log($"host：客户端 {peer.Address}:{peer.Port} 断开连接。原因：{disconnectInfo.Reason}");

            

            OnDisconnected?.Invoke(peer, disconnectInfo);
        };
    }

    public void PollEvents()
    {
        _server?.PollEvents();
    }

    public void Stop()
    {
        _server?.Stop();
        RoomManager.Instance.Clear();
    }
}
