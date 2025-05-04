
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

        public bool IsDummy { get; set; }

        public bool IsDead  { get; set; }//角色死亡动画之后设为true

        public float PlayerHeight { get; set; }
        public float PlayerDeadHeight { get; set; }

        public BasicPlayerInfo()
        {
            AppearanceData = new PlayerAppearanceData();
            LV = 1;
            Name = "Player";
            Position = Vector3.zero;
            MaxLife = 100;
            Life = 100;
            MaxPower = 100;
            Power = 100;
            IsDummy = false;
            PlayerHeight = 2.29f;
            PlayerDeadHeight = 0.2f;
        }
    } 
}
