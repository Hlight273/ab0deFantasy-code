using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerEntity: BaseEntity
    {
        public GameObject GameObject { get; private set; }

        public GameObject PlayerObject { get; private set; }

        public BasicPlayerInfo Info { get ; private set ; }

        public PlayerEntity(int id, BasicPlayerInfo info, GameObject gameObject)
        {
            GameObject = gameObject;
            Info = info;
            Id = id;
            PlayerObject = gameObject.transform.GetChild(0).gameObject; 
        }

        public PlayerEntity()
        {
        }
    }
}