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

            // ��������
            if (Random.value < targetStats.DodgeChance)
            {
                return builder.WithDodge().Build();
            }

            // �����м�
            if (Random.value < targetStats.ParryChance)
            {
                return builder.WithParry().Build();
            }

            // ������
            if (Random.value < attackerStats.CriticalChance)
            {
                builder.WithCritical(attackerStats.CriticalDamage);
            }

            // �����
            if (Random.value < targetStats.BlockChance)
            {
                builder.WithBlock(targetStats.DamageReduction);
            }

            // ��������
            if (targetStats.DamageReduction > 0)
            {
                builder.WithAbsorption(targetStats.DamageReduction);
            }

            // ��������͵ȡ
            if (attackerStats.LifeSteal > 0)
            {
                builder.WithLifeSteal(attackerStats.LifeSteal);
            }

            // �����˺�����Ӧ�÷���
            switch (damageType)
            {
                case DamageType.Physical:
                    ApplyPhysicalDefense(builder, attackerStats, targetStats);
                    break;
                case DamageType.Magic:
                    ApplyMagicDefense(builder, attackerStats, targetStats);
                    break;
                case DamageType.True:
                    // ��ʵ�˺����ܷ���Ӱ��
                    break;
                case DamageType.Pure:
                    // �����˺������κμӳɺͼ���Ӱ��
                    break;
            }

            // Ӧ��ȫ���˺�����
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