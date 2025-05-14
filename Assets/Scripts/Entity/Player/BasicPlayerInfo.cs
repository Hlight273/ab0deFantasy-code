
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    [System.Serializable]
    public class BasicPlayerInfo
    {
        public PlayerAppearanceData AppearanceData;
        public int LV;
        public string Name;
        public Vector3 Position;
        public int MaxLife;

        public int Life;

        public int MaxPower;

        public int Power;

        public bool IsDummy { get; set; }

        public bool IsDead;//角色死亡动画之后设为true

        public float PlayerHeight;

        public float PlayerViewHeight;//血条位置

        public float PlayerDeadHeight;

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
            PlayerViewHeight = 2f;
            PlayerDeadHeight = 0.2f;
        }
    } 
}
