namespace HFantasy.Script.Network.Config
{
    public static class CommonNetConfig
    {
#if UNITY_EDITOR
        public const int DefaultPort = 7779;
#else
        public const int DefaultPort = 7779;
#endif

        public const int DefaultUdpPort = 43333;
        public const int MaxPlayerCount = 4;
        public const int MaxRoomCount = 10;
        
        public const string ProtocolVersion = "1.0.0";
        
        public const float ConnectionTimeout = 5f;
        public const float PingInterval = 2f;
        
        public const string DiscoverMessage = "DISCOVER_ROOM";
    }
}