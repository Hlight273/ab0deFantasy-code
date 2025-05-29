
namespace HFantasy.Script.Network.Protocol
{
    [System.Serializable]
    public class RoomInfo
    {
        public string ip;
        public int port;
        public string name;
        public int curPlayerCount;
        public int maxPlayerCount;
    }
}
