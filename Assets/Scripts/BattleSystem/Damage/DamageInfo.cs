using UnityEngine;

namespace HFantasy.Script.BattleSystem.Damage
{
    public class DamageInfo
    {
        // ������Ϣ
        public int AttackerId { get; private set; }
        public int TargetId { get; private set; }
        public Vector3 HitPoint { get; private set; }
        public Vector3 HitDirection { get; private set; }

        //�˺�����
        public DamageType DamageType { get; private set; }
        public DamageModifier Modifiers { get; set; }

        //�˺���ֵ
        public float BaseDamage { get; private set; }        //�����˺�
        public float CriticalMultiplier { get; set; }        //��������
        public float BlockReduction { get; set; }            //�񵲼���
        public float AbsorptionAmount { get; set; }          //������
        public float FinalDamage { get; private set; }       //�����˺�

        //ս������
        public float ArmorPenetration { get; set; }          //���״�͸
        public float MagicPenetration { get; set; }          //������͸
        public float LifeSteal { get; set; }                 //����͵ȡ

        public DamageInfo(int attackerId, int targetId, float baseDamage, DamageType type, Vector3 hitPoint, Vector3 hitDirection)
        {
            AttackerId = attackerId;
            TargetId = targetId;
            BaseDamage = baseDamage;
            DamageType = type;
            HitPoint = hitPoint;
            HitDirection = hitDirection;

            //Ĭ��ֵ
            Modifiers = DamageModifier.None;
            CriticalMultiplier = 1.0f;
            BlockReduction = 0f;
            AbsorptionAmount = 0f;
            FinalDamage = baseDamage;
        }

        public void CalculateFinalDamage()
        {
            float damage = BaseDamage;

            //����
            if ((Modifiers & DamageModifier.Critical) != 0)
            {
                damage *= CriticalMultiplier;
            }

            //��
            if ((Modifiers & DamageModifier.Blocked) != 0)
            {
                damage *= (1 - BlockReduction);
            }

            //����
            if ((Modifiers & DamageModifier.Dodged) != 0)
            {
                damage = 0;
            }

            //�м�
            if ((Modifiers & DamageModifier.Parried) != 0)
            {
                damage = 0;
            }

            //����
            if ((Modifiers & DamageModifier.Absorbed) != 0)
            {
                damage = Mathf.Max(0, damage - AbsorptionAmount);
            }

            FinalDamage = damage;
        }

        public class Builder
        {
            private readonly DamageInfo damage;

            public Builder(int attackerId, int targetId, float baseDamage)
            {
                damage = new DamageInfo(
                    attackerId,
                    targetId,
                    baseDamage,
                    DamageType.Physical, // Ĭ�������˺�
                    Vector3.zero,        // Ĭ����ײ��
                    Vector3.zero         // Ĭ�Ϸ���
                );
            }

            public Builder WithDamageType(DamageType type)
            {
                damage.DamageType = type;
                return this;
            }

            public Builder WithDefenseModifier(float defenseModifier)
            {
                damage.BaseDamage *= defenseModifier;
                return this;
            }

            public Builder WithCritical(float multiplier)
            {
                damage.Modifiers |= DamageModifier.Critical;
                damage.CriticalMultiplier = multiplier;
                return this;
            }

            public Builder WithBlock(float reduction)
            {
                damage.Modifiers |= DamageModifier.Blocked;
                damage.BlockReduction = reduction;
                return this;
            }

            public Builder WithDodge()
            {
                damage.Modifiers |= DamageModifier.Dodged;
                return this;
            }

            public Builder WithParry()
            {
                damage.Modifiers |= DamageModifier.Parried;
                return this;
            }

            public Builder WithAbsorption(float amount)
            {
                damage.Modifiers |= DamageModifier.Absorbed;
                damage.AbsorptionAmount = amount;
                return this;
            }

            public Builder WithLifeSteal(float amount)
            {
                damage.LifeSteal = amount;
                return this;
            }

            public Builder WithHitInfo(Vector3 hitPoint, Vector3 hitDirection)
            {
                damage.HitPoint = hitPoint;
                damage.HitDirection = hitDirection;
                return this;
            }

            public DamageInfo Build()
            {
                damage.CalculateFinalDamage();
                return damage;
            }
        }
    }

    public enum DamageType
    {
        Physical,
        Magic,
        True,    //��ʵ�˺�
        Pure     //�����˺�
    }

    [System.Flags]
    public enum DamageModifier
    {
        None = 0,
        Critical = 1 << 0,
        Blocked = 1 << 1,
        Dodged = 1 << 2,
        Parried = 1 << 3,
        Absorbed = 1 << 4
    }

    public class CombatStats
    {
        //��������
        public float PhysicalDefense { get; set; }
        public float MagicResistance { get; set; }
        public float BlockChance { get; set; }
        public float DodgeChance { get; set; }
        public float ParryChance { get; set; }

        //��������
        public float CriticalChance { get; set; }
        public float CriticalDamage { get; set; }
        public float PhysicalPenetration { get; set; }
        public float MagicPenetration { get; set; }
        public float LifeSteal { get; set; }

        //��������
        public float DamageReduction { get; set; }
        public float DamageAmplification { get; set; }
    }
}