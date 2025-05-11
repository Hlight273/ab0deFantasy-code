using HFantasy.Script.Common.Constant;
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerBattleInfo
    {
        public float AttackSpeed { get; set; }//攻速属性
        public float AttackInterval { get; set; } //最终攻击间隔
        public float BaseAttackSpeed { get; set; }  //基础攻速（武器等影响）

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

        public float AttackSpeedToInterval(float attackSpeed)
        {
            const float a = 0.09f;
            const float b = 1f;

            float denominator = Mathf.Max(a * attackSpeed + b, 0.1f);
            return (1f / denominator) * BaseAttackSpeed;
        }

        public float IntervalToAttackSpeed(float interval)
        {
            const float a = 0.09f;
            const float b = 1f;

            float normalizedInterval = interval / BaseAttackSpeed;
            return (1f / normalizedInterval - b) / a;
        }

        public PlayerBattleInfo(int lv)
        {
            BaseAttackSpeed = 1f;  //默认基础攻速为1
            AttackSpeed = 20f;
            AttackInterval = AttackSpeedToInterval(AttackSpeed);
            AttackRange = 1.0f;
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

        public float GetAnimationSpeedMultiplier()
        {
            if(AttackSpeed <= 0f)
            {
                return 1f;
            }
            //基础动画速度为1，攻速100%时动画速度为10倍，得到的动画倍率为1+攻速/10
            return 1f + (AttackSpeed / 10f);
        }


    }
}