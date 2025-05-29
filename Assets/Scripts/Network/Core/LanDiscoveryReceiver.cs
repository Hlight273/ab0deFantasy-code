using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using HFantasy.Script.Network.Protocol;
using HFantasy.Script.Network.Config;
using HFantasy.Script.Clinet;

public class LanDiscoveryReceiver
{
    private UdpClient _udpListener;
    private int _listenPort = CommonNetConfig.DefaultUdpPort;

    LiteNetLibServer _server;

    public LanDiscoveryReceiver(LiteNetLibServer _server)
    {
        this._server = _server;
    }

    public void StartListening()
    {
        _udpListener = new UdpClient(_listenPort);
        RoomInfo roomInfo = new RoomInfo
        {
            ip = GetLocalIP(),
            name = "My Room",
            port = CommonNetConfig.DefaultPort,
            curPlayerCount = _server.GetCurrentPlayerCount(),
            maxPlayerCount = CommonNetConfig.MaxPlayerCount,
        };
        MainContext.Instance.RoomContext.MyRoomInfo = roomInfo;

        Task.Run(async () =>
        {
            while (true)
            {
                var result = await _udpListener.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);
                if (msg == "DISCOVER_ROOM")
                {
                    roomInfo = new RoomInfo
                    {
                        ip = GetLocalIP(),
                        name = "My Room",
                        port = CommonNetConfig.DefaultPort,
                        curPlayerCount = _server.GetCurrentPlayerCount(),
                        maxPlayerCount = CommonNetConfig.MaxPlayerCount,
                    };
                    MainContext.Instance.RoomContext.MyRoomInfo = roomInfo;
                    string response = JsonUtility.ToJson(roomInfo);

                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await _udpListener.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);
                }
            }
        });
    }

    private string GetLocalIP()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
                  .AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                  ?.ToString() ?? "127.0.0.1";
    }
}
