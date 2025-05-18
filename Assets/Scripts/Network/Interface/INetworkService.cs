using System;

namespace HFantasy.Script.Network.Interface
{
    public interface INetworkService
    {
        bool IsConnected { get; }
        bool IsHost { get; }
        void Initialize();
        void StartHost(int port);
        void StartClient(string address, int port);
        void Disconnect();
        void SendMessage(INetworkMessage message);
        void RegisterHandler<T>(Action<T> handler) where T : class, INetworkMessage, new();
        void UnregisterHandler<T>(Action<T> handler) where T : class, INetworkMessage, new();
    }
}