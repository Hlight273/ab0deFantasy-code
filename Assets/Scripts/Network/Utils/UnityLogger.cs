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
                Debug.Log("[利大log] " + message);
                break;
            case NetLogLevel.Warning:
                Debug.LogWarning("[利大log] " + message);
                break;
            case NetLogLevel.Error:
                Debug.LogError("[利大log] " + message);
                break;
        }
    }
}
