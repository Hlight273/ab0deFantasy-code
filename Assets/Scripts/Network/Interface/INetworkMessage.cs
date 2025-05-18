using LiteNetLib.Utils;

namespace HFantasy.Script.Network.Interface
{
    public interface INetworkMessage : INetSerializable
    {
        byte MessageType { get; }
    }
}