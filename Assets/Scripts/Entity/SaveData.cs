using HFantasy.Script.Entity.Player;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Entity
{
    [System.Serializable]
    public class SaveData
    {
        public BasicPlayerInfo myPlayerInfo;
        public string lastSaveTime;

        public SaveData()
        {
         
        }

    }
}


