using HFantasy.Script.Common.Constant;
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerBattleInfo
    {
        public float AttackSpeed { get; set; }

        public float AttackRange { get; set; }
        public float AttackForwardDis { get; set; }
        public float AttackHeightDis { get; set; }
        public float PhysicDamage { get; set; }
        public float MagicDamage { get; set; }
        public float Crit { get; set; }
        public float CritDamage { get; set; }

        public float PhysicDefence { get; set; }
        public float MagicDefence { get; set; }

        public float FinalDamageRate { get; set; }
        public float FinalReduceDmgRate { get; set; }


        public PlayerBattleInfo(int lv)
        {
            AttackSpeed = 10f;
            AttackRange = 0.5f;
            AttackHeightDis = 0.8f;
            AttackForwardDis = 0.25f;
            PhysicDamage = 10f+ lv;
            MagicDamage = 10f+ lv;
            Crit = 10f;
            CritDamage = 100f;
            PhysicDefence = 10f;
            MagicDefence = 10f;
            FinalDamageRate = 0f;
            FinalReduceDmgRate = 0f;
        }

        
    }
}