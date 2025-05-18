using HFantasy.Script.Network.Core;
using HFantasy.Script.Network.Room;
using UnityEngine;

namespace HFantasy.Script.Network
{
    public class NetworkBootstrap : MonoBehaviour
    {
        [SerializeField] private int defaultPort = 7777;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            NetworkManager.Instance.Initialize();
            RoomManager.Instance.Initialize();
        }

        private void Update()
        {
            NetworkManager.Instance.Update();
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.Dispose();
            RoomManager.Instance.Dispose();
        }
    }
}