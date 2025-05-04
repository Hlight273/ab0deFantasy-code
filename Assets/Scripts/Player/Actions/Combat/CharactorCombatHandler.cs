namespace HFantasy.Script.Player.Actions.Combat
{
    using HFantasy.Script.BattleSystem.Damage;
    using HFantasy.Script.BattleSystem.Events;
    using HFantasy.Script.Common.Constant;
    using HFantasy.Script.Core;
    using HFantasy.Script.Core.Events;
    using HFantasy.Script.Entity.Player;
    using UnityEngine;

    public class CharactorCombatHandler : ICharactorCombat
    {
        private readonly CharacterController controller;
        private readonly Animator animator;

        private PlayerEntity playerEntity;
        public PlayerEntity PlayerEntity => playerEntity;

        public Vector3 AttackCenter
        {
            get
            {
                if (controller == null) return Vector3.zero;
                Vector3 finalPos = controller.transform.position ;
                finalPos += controller.transform.forward * playerEntity.RuntimeInfo.BattleInfo.AttackForwardDis;
                finalPos += controller.transform.up * playerEntity.RuntimeInfo.BattleInfo.AttackHeightDis;
                return finalPos;
            }
        }


        public bool IsAttacking => playerEntity?.RuntimeInfo?.AttackTimer > 0;
        public bool IsCombatState => playerEntity?.RuntimeInfo?.CombatStateTimer > 0;

        public bool AttackPressed => InputManager.Instance.AttackPressed;

        public bool CanAttack => AttackPressed && playerEntity != null && playerEntity.RuntimeInfo.CanAttack;

        public bool IsAttackRecovering => playerEntity?.RuntimeInfo?.AttackRecoveryTimer > 0;

        public CharactorCombatHandler(PlayerEntity playerEntity, CharacterController controller, Animator animator)
        {
            this.playerEntity = playerEntity;
            this.controller = controller;
            this.animator = animator;

        }

        public void PerformAttack()
        {
            if (playerEntity == null || playerEntity.RuntimeInfo == null) return;
            playerEntity.RuntimeInfo.AttackTimer = playerEntity.RuntimeInfo.attackDuration;//恢复攻击计时和战斗计时
            playerEntity.RuntimeInfo.CombatStateTimer = playerEntity.RuntimeInfo.combatStateDuration;
            playerEntity.RuntimeInfo.AttackRecoveryTimer = playerEntity.RuntimeInfo.attackRecoveryDuration;

            //发布攻击开始事件
            EventManager.Instance.Publish(new CombatEventData(
                CombatEventData.EventType.AttackStart,
                controller.gameObject
            ));

            //检测命中
            //CheckHit();
        }

        public void OnAttackHitFrame()
        {
            CheckHit();
        }

        private void CheckHit()
        {
            if (playerEntity == null || playerEntity.RuntimeInfo == null) return;
            Collider[] hitColliders = Physics.OverlapSphere(AttackCenter, 
                playerEntity.RuntimeInfo.BattleInfo.AttackRange, 
                LayerConstant.Masks.CanPlayerAttackMask);//需要检查角色攻击范围属性, 在LayerConstant.Masks中有配置可以被碰撞检测攻击的层

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject == controller.gameObject) continue;

                var damageable = hitCollider.GetComponent<PlayerActions>().PlayerEntity;
                if (damageable != null && !damageable.Info.IsDead)
                {
                    var damageInfo = new DamageInfo.Builder(
                        playerEntity.Id,
                        damageable.Id,
                        playerEntity.RuntimeInfo.BattleInfo.PhysicDamage//需要检查角色攻击力属性，关于物理和魔法 以后再扩展
                    )
                    .WithHitInfo(
                        hitCollider.ClosestPoint(AttackCenter),
                        (hitCollider.transform.position - controller.transform.position).normalized
                    )
                    .Build();

                    //发布命中事件
                    EventManager.Instance.Publish(new CombatEventData(
                        CombatEventData.EventType.AttackHit,
                        controller.gameObject,
                        hitCollider.gameObject,
                        damageInfo
                    ));

                    damageable.TakeDamage(damageInfo);
                }
            }
        }

    }
}