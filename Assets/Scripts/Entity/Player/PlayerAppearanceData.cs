using Newtonsoft.Json;
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    [System.Serializable]
    public class PlayerAppearanceData
    {
        public int BodyId;
        public int HairId;
        public int ArmorId;

        public Color SkinColor;
        public Color HairColor;

        public PlayerAppearanceData()
        {
            BodyId = 1;
            HairId = 1;
            ArmorId = 1;
            SkinColor = Color.white;
            HairColor = Color.white;
        }
    }
}