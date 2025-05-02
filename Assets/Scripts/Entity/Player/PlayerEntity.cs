using HFantasy.Script.Player.Movement;
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerEntity: BaseEntity
    {
        public GameObject GameObject { get; private set; }

        public GameObject PlayerObject { get; private set; }

        public BasicPlayerInfo Info { get ; private set ; }

        public bool isLocalPlayer { get; private set; }

        public PlayerEntity(int id, BasicPlayerInfo info, GameObject gameObject, bool isLocalPlayer)
        {
            GameObject = gameObject;
            Info = info;
            Id = id;
            PlayerObject = gameObject.transform.GetChild(0).gameObject;
            this.isLocalPlayer = isLocalPlayer;
            PlayerMovement movement = PlayerObject.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.IsLocalPlayer = isLocalPlayer;
            }
            else
            {
                Debug.LogError($"PlayerMovement×é¼þÎ´ÕÒµ½£¬ID: {id}");
            }
        }

        public PlayerEntity()
        {
        }
    }
}