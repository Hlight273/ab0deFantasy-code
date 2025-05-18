using LiteNetLib;
using LiteNetLib.Utils;
using HFantasy.Script.Network.Interface;
using HFantasy.Script.Network.Protocol;
using HFantasy.Script.Common;
using System;
using UnityEngine;

namespace HFantasy.Script.Network.Core
{
    public class NetworkManager : Singleton<NetworkManager>, INetEventListener, INetworkService, IDisposable
    {
        private NetManager netManager;
        private NetPeer serverPeer;
        private NetPacketProcessor packetProcessor;
        private bool isHost;
        private bool initialized;

        public bool IsConnected => serverPeer?.ConnectionState == ConnectionState.Connected;
        public bool IsHost => isHost;

        public void Initialize()
        {
            if (initialized) return;
            InitializeNetwork();
            initialized = true;
        }

        private void InitializeNetwork()
        {
            packetProcessor = new NetPacketProcessor();
            RegisterPacketHandlers();
        }

        private void RegisterPacketHandlers()
        {
            //注册所有消息类型
            packetProcessor.RegisterNestedType(() => new RoomInfo());
            packetProcessor.RegisterNestedType(() => new CreateRoomMessage());
            packetProcessor.RegisterNestedType(() => new JoinRoomMessage());
            packetProcessor.RegisterNestedType(() => new LeaveRoomMessage());
            packetProcessor.RegisterNestedType(() => new RoomListMessage());
            packetProcessor.RegisterNestedType(() => new JoinRoomResultMessage());
            packetProcessor.RegisterNestedType(() => new LeaveRoomResultMessage());
            packetProcessor.RegisterNestedType(() => new RequestRoomListMessage());
        }

        public void RegisterHandler<T>(Action<T> handler) where T : class, INetworkMessage, new()
        {
            packetProcessor.SubscribeReusable(handler);
        }

        public void UnregisterHandler<T>(Action<T> handler) where T : class, INetworkMessage, new()
        {
            packetProcessor.RemoveSubscription<T>();
        }

       public void StartHost(int port)
{
    if (!initialized)
    {
        Debug.LogError("NetworkManager not initialized!");
        return;
    }

    netManager = new NetManager(this)
    {
        UnconnectedMessagesEnabled = true,
        UpdateTime = 15
    };

    if (netManager.Start(port))
    {
        isHost = true;
        // 作为主机时，不需要连接到自己
        // StartClient("localhost", port);
        Debug.Log($"Host started on port {port}");
    }
}

        public void StartClient(string address, int port)
        {
            if (!initialized)
            {
                Debug.LogError("NetworkManager not initialized!");
                return;
            }

            netManager = new NetManager(this)
            {
                UpdateTime = 15
            };

            if (netManager.Start())
            {
                netManager.Connect(address, port, "game");
                Debug.Log($"Connecting to {address}:{port}");
            }
        }

        public void Disconnect()
        {
            netManager?.Stop();
            serverPeer = null;
            isHost = false;
        }

        public void SendMessage(INetworkMessage message)
        {
            if (!initialized || serverPeer?.ConnectionState != ConnectionState.Connected) return;

            var writer = new NetDataWriter();
            message.Serialize(writer);
            serverPeer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void Update()
        {
            if (!initialized) return;
            netManager?.PollEvents();
        }

        public void Dispose()
        {
            Disconnect();
            initialized = false;
        }

        #region INetEventListener Implementation
        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"Connected to: {peer.Address}:{peer.Port}");
            serverPeer = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"Disconnected: {disconnectInfo.Reason}");
            if (peer == serverPeer)
                serverPeer = null;
        }

        public void OnNetworkError(System.Net.IPEndPoint endPoint, System.Net.Sockets.SocketError socketError)
        {
            Debug.LogError($"Network error: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            try
            {
                packetProcessor.ReadAllPackets(reader, peer);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing packet: {e}");
            }
        }

        public void OnNetworkReceiveUnconnected(System.Net.IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnConnectionRequest(ConnectionRequest request) { request.Accept(); }
        #endregion
    }
}