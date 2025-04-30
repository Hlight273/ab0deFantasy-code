
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    [System.Serializable]
    public class BasicPlayerInfo
    {
        public PlayerAppearanceData AppearanceData;
        public int LV { get; set; }
        public string Name { get;  set; }
        public Vector3 Position { get;  set; }
        public int MaxLife { get;  set; }

        public int Life { get;  set; }

        public int MaxPower { get;  set; }

        public int Power { get;  set; }
    } 
}
