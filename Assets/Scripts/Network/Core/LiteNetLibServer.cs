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

    //RoomManager�淿��������Ϣ
    public IReadOnlyCollection<NetPlayerInfo> GetConnectedPlayers() => RoomManager.Instance.GetAllPlayers();

    public int GetCurrentPlayerCount() => _server?.ConnectedPeersCount ?? -1;

    public void StartServer(int port)
    {
        _listener = new EventBasedNetListener();
        _server = new NetManager(_listener);
        bool started = _server.Start(port);
        if (!started)
        {
            Debug.LogError("����������ʧ�ܣ�");
        }

        _listener.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey("room_key");
        };

        _listener.PeerConnectedEvent += peer =>
        {
            Debug.Log($"host���ͻ��� {peer.Address}:{peer.Port} �������ӵ��� �����˿� {_server.LocalPort}���ȴ����������Ϣ");
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
                        Debug.Log($"��� {joinReq.BasicPlayerInfo.Name} ������� {joinReq.RoomId}");

                        var netPlayer = new NetPlayerInfo(peer, joinReq.BasicPlayerInfo);
                        RoomManager.Instance.AddPlayer(netPlayer);

                        //�ظ��ͻ��˼���ɹ�
                        var response = new JoinRoomResponse
                        {
                            Success = true,
                            Message = "����ɹ�",
                            RemoteInitPlayerInfos = EntityManager.Instance.GetRemoteInitPlayerInfos()
                        };

                        var responseData = NetMessageHelper.Pack(NetMessageType.JoinRoomResponse, response);
                        peer.Send(responseData, DeliveryMethod.ReliableOrdered);

                        //�㲥��Ҽ���
                        RoomManager.Instance.BroadcastPlayerJoined(netPlayer);
                        break;
                    }
            }
        };

        _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            RoomManager.Instance.BroadcastPlayerExit(peer);
            RoomManager.Instance.RemovePlayer(peer);
            Debug.Log($"host���ͻ��� {peer.Address}:{peer.Port} �Ͽ����ӡ�ԭ��{disconnectInfo.Reason}");

            

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
