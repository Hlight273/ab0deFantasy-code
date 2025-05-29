using HFantasy.Script.Entity.Player;
using LiteNetLib;

public class NetPlayerInfo
{
    public NetPeer Peer { get; private set; }
    public BasicPlayerInfo BasicInfo { get; private set; }



    public NetPlayerInfo(NetPeer peer, BasicPlayerInfo basicInfo)
    {
        Peer = peer;
        BasicInfo = basicInfo;
    }

    public string Name => BasicInfo?.Name ?? $"Peer_{Peer?.Id}";
}
