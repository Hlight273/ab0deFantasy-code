using HFantasy.Script.Common;
using HFantasy.Script.Network.Config;
using LiteNetLib;
using System;
using UnityEngine;

namespace HFantasy.Script.Network
{

    public class GameNetworkManager : MonoSingleton<GameNetworkManager>
    {
        private LiteNetLibServer _server;
        private LiteNetLibClient _client;
        private LanDiscoveryReceiver _receiver;
        private LanDiscoveryClient _discoverClient;

        public Action<NetPeer> OnClientConnected;
        public Action<NetPeer, DisconnectInfo> OnClientDisconnected;
        public Action<NetPeer> OnHostRecieveConnected;
        public Action<NetPeer, DisconnectInfo> OnHostRecieveDisconnected;

        private void Start()
        {
            NetDebug.Logger = new UnityLogger();
            Application.runInBackground = true;
        }

        void Update()
        {
            _client?.PollEvents();
            _server?.PollEvents();
        }

        public void CreateRoomNetAction()
        {
            _server = new LiteNetLibServer();
            _server.OnConnected += peer => {
                OnHostRecieveConnected?.Invoke(peer);
            };
            _server.OnDisconnected += (peer, disconnectInfo) => {
                OnHostRecieveDisconnected?.Invoke(peer, disconnectInfo);
            };
            _server.StartServer(CommonNetConfig.DefaultPort);


            _receiver = new LanDiscoveryReceiver(_server);
            _receiver.StartListening();

            _client = new LiteNetLibClient();
            _client.OnConnected += peer => {
                OnClientConnected?.Invoke(peer);
            };
            _client.OnDiscnnected += (peer, disconnectInfo) => {
                OnClientDisconnected?.Invoke(peer,disconnectInfo);
            };
            _client.StartClient("127.0.0.1", CommonNetConfig.DefaultPort); //Host 自己连自己
        }

        public void JoinLobbyNetAction()
        {
            _discoverClient = new LanDiscoveryClient();
            _discoverClient.Start();//开始房间的广播和监听
        }
        public void StopJoinLobbyNetAction()
        {
            _discoverClient?.Stop();
            _discoverClient = null;

        }

        public void JoinRoomNetAction(string ip, int port)
        {
            _client = new LiteNetLibClient();
            _client.OnConnected += peer => {
                OnClientConnected?.Invoke(peer);
            };
            _client.OnDiscnnected += (peer, disconnectInfo) => {
                OnClientDisconnected?.Invoke(peer, disconnectInfo);
            };
            _client.StartClient(ip, port);
        }
    }
}