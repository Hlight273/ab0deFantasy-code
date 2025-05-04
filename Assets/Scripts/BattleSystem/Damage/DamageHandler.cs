using UnityEngine;

namespace HFantasy.Script.BattleSystem.Damage
{
    public class DamageHandler
    {
        public static DamageInfo ProcessDamage(int attackerId, int targetId, float baseDamage,
            CombatStats attackerStats, CombatStats targetStats, DamageType damageType,
            Vector3 hitPoint, Vector3 hitDirection)
        {
            var builder = new DamageInfo.Builder(attackerId, targetId, baseDamage)
                .WithDamageType(damageType)
                .WithHitInfo(hitPoint, hitDirection);

            // 处理闪避
            if (Random.value < targetStats.DodgeChance)
            {
                return builder.WithDodge().Build();
            }

            // 处理招架
            if (Random.value < targetStats.ParryChance)
            {
                return builder.WithParry().Build();
            }

            // 处理暴击
            if (Random.value < attackerStats.CriticalChance)
            {
                builder.WithCritical(attackerStats.CriticalDamage);
            }

            // 处理格挡
            if (Random.value < targetStats.BlockChance)
            {
                builder.WithBlock(targetStats.DamageReduction);
            }

            // 处理吸收
            if (targetStats.DamageReduction > 0)
            {
                builder.WithAbsorption(targetStats.DamageReduction);
            }

            // 处理生命偷取
            if (attackerStats.LifeSteal > 0)
            {
                builder.WithLifeSteal(attackerStats.LifeSteal);
            }

            // 根据伤害类型应用防御
            switch (damageType)
            {
                case DamageType.Physical:
                    ApplyPhysicalDefense(builder, attackerStats, targetStats);
                    break;
                case DamageType.Magic:
                    ApplyMagicDefense(builder, attackerStats, targetStats);
                    break;
                case DamageType.True:
                    // 真实伤害不受防御影响
                    break;
                case DamageType.Pure:
                    // 纯粹伤害不受任何加成和减免影响
                    break;
            }

            // 应用全局伤害修正
            float finalModifier = (1 + attackerStats.DamageAmplification) * (1 - targetStats.DamageReduction);
            builder.WithDefenseModifier(finalModifier);

            return builder.Build();
        }

        private static void ApplyPhysicalDefense(DamageInfo.Builder builder, CombatStats attackerStats, CombatStats targetStats)
        {
            float effectiveDefense = Mathf.Max(0, targetStats.PhysicalDefense - attackerStats.PhysicalPenetration);
            float defenseModifier = 100f / (100f + effectiveDefense);
            builder.WithDefenseModifier(defenseModifier);
        }

        private static void ApplyMagicDefense(DamageInfo.Builder builder, CombatStats attackerStats, CombatStats targetStats)
        {
            float effectiveResistance = Mathf.Max(0, targetStats.MagicResistance - attackerStats.MagicPenetration);
            float resistanceModifier = 100f / (100f + effectiveResistance);
            builder.WithDefenseModifier(resistanceModifier);
        }
    }
}