using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

public class UnityLogger : INetLogger
{
    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        string message = string.Format(str, args);
        switch (level)
        {
            case NetLogLevel.Info:
                Debug.Log("[����log] " + message);
                break;
            case NetLogLevel.Warning:
                Debug.LogWarning("[����log] " + message);
                break;
            case NetLogLevel.Error:
                Debug.LogError("[����log] " + message);
                break;
        }
    }
}
