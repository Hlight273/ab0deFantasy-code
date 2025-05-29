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
        Debug.Log("�ͻ��������ɹ���" + success);
        NetPeer netPeer = _client.Connect(ip, port, "room_key");
        Debug.Log("�ͻ������ӳɹ���" + netPeer.Address+":"+netPeer.Port);

        _listener.PeerConnectedEvent += peer =>
        {
            _connectedPeer = peer;
            Debug.Log($"�ͻ������ӵ�������: {peer.Address}:{peer.Port}");
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
                    Debug.Log($"���뷿����: {(res.Success ? "�ɹ�" : "ʧ��")} - {res.Message}");
                    if (res.Success)
                    {
                        //���뷿��ɹ������ǲ���ֱ�Ӵ����������ʵ�壬Ҫ�ڳ���������ɺ��ٴ���
                        //EntityManager.Instance.CreateRemotePlayers(res.RemoteInitPlayerInfos);
                        MainContext.Instance.RemoteInitPlayerInfos = res.RemoteInitPlayerInfos;
                        //֪ͨ��������Ҽ���ɹ�
                        //var joinBroadcast = new JoinRoomRequest { BasicPlayerInfo = res.BasicPlayerInfo };
                        //_client.SendToAll(NetMessageHelper.Pack(NetMessageType.PlayerJoinedBroadcast, joinBroadcast));
                    }
                    else
                    {
                        //����ʧ��
                        Debug.LogError("���뷿��ʧ��: " + res.Message);
                    }
                    break;
                case NetMessageType.PlayerJoinedBroadcast:
                    {
                        var data = NetMessageHelper.DecodePayload<JoinRoomRequest>(msg);
                        Debug.Log($"�յ���Ҽ���㲥��{data.BasicPlayerInfo.Name}");

                        //����Զ�����
                        PlayerEntity playerEntity = EntityManager.Instance.CreatePlayerEntity(data.BasicPlayerInfo, false, SceneConstant.ThreeDLobby, peer.Id);
                        break;
                    }
                case NetMessageType.PlayeExitBroadcast:
                    {
                        var data = NetMessageHelper.DecodePayload<NetPlayerInfo>(msg);
                        Debug.Log($"�յ�����뿪�㲥��{data.Name}");
                        //�Ƴ�Զ�����
                        EntityManager.Instance.RemovePlayerEntityByNetId(data.Peer.Id);
                        break;
                    }
            }
        };

        _listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
        {
            Debug.LogWarning($"�ͻ�����������Ͽ�����: {peer.Address}:{peer.Port}, ԭ��: {disconnectInfo.Reason}");
            _connectedPeer = null;
            OnDiscnnected?.Invoke(peer, disconnectInfo);
        };
        _listener.NetworkErrorEvent += (endPoint, error) =>
        {
            Debug.LogError($"�ͻ������ӷ������������ {endPoint}: {error}");
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
