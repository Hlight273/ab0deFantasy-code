using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using HFantasy.Script.Network.Protocol;
using HFantasy.Script.Network.Config;
using HFantasy.Script.Clinet;

public class LanDiscoveryClient
{
    private UdpClient _udpClient;
    private IPEndPoint _broadcastEP;

    private CancellationTokenSource _cts;

    public LanDiscoveryClient()
    {
        _udpClient = new UdpClient(0);
        _udpClient.EnableBroadcast = true;
        _broadcastEP = new IPEndPoint(IPAddress.Broadcast, CommonNetConfig.DefaultUdpPort);
        _cts = new CancellationTokenSource();
    }

    public void Start()
    {
        Debug.Log($"[LanDiscovery] ����ʹ�ö˿ڣ�{_udpClient.Client.LocalEndPoint}");
        StartBroadcastLoop(_cts.Token);
        StartListenLoop(_cts.Token);
    }

    private async void StartBroadcastLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                byte[] msg = Encoding.UTF8.GetBytes("DISCOVER_ROOM");
                await _udpClient.SendAsync(msg, msg.Length, _broadcastEP);
                await Task.Delay(2000, token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("[LanDiscovery] �㲥ѭ��ֹͣ");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LanDiscovery] �㲥����: {ex}");
        }
    }

    private async void StartListenLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                try
                {
                    RoomInfo info = JsonUtility.FromJson<RoomInfo>(msg);
                    if (info != null && !string.IsNullOrEmpty(info.ip))
                    {
                        Debug.Log($"[LanDiscovery] �յ�����: {info.ip}:{info.port} ����: {info.curPlayerCount}");
                        MainContext.Instance.RoomContext.AddOrUpdateRoom(info);
                    }
                }
                catch
                {
                    Debug.LogWarning("[LanDiscovery] �յ���Ч����: " + msg);
                }
            }
        }
        catch (ObjectDisposedException)
        {
            Debug.Log("[LanDiscovery] �������ѹر�");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LanDiscovery] ���մ���: {ex}");
        }
    }

    public void Stop()
    {
        Debug.Log("[LanDiscovery] ֹͣ�㲥�����");
        _cts.Cancel();
        _udpClient?.Close(); // �ͷŶ˿���Դ
        _udpClient = null;
    }
}
