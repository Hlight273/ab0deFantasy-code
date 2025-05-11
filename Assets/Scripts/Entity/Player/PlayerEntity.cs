using HFantasy.Script.BattleSystem.Damage;
using HFantasy.Script.BattleSystem.Events;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Core;
using HFantasy.Script.Core.Events;
using HFantasy.Script.Player.Actions;
using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerEntity : BaseEntity, IDamageable
    {
        public GameObject GameObject { get; private set; }

        public GameObject PlayerObject { get; private set; }

        public BasicPlayerInfo Info { get ; private set ; }
        public PlayerRuntimeInfo RuntimeInfo { get; private set; }

        private PlayerActions actions;

        public bool isLocalPlayer { get; private set; }

        public PlayerEntity(int id, BasicPlayerInfo info, GameObject gameObject, bool isLocalPlayer)
        {
            GameObject = gameObject;
            Info = info;
            RuntimeInfo = new PlayerRuntimeInfo(info.LV);
            Id = id;
            PlayerObject = gameObject.transform.GetChild(0).gameObject;
            this.isLocalPlayer = isLocalPlayer;
            actions = PlayerObject.GetComponent<PlayerActions>();
            if (actions != null)
            {
                actions.InitializePlayerEntity(this);
            }
            else
            {
                Debug.LogError($"PlayerActions，ID: {id}");
            }
            EventManager.Instance.Subscribe<CombatEventData>(HandleCombatEvent);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            int finalDamage = (int)damageInfo.FinalDamage;
            Info.Life -= finalDamage;
            if (Info.Life <= 0)//发布死亡时间和受伤互斥
            {
                Die(damageInfo);
                return;
            }
            else
            {
                //发布受伤事件
                EventManager.Instance.Publish(new CombatEventData(
                    CombatEventData.EventType.DamageTaken,
                    EntityManager.Instance.GetPlayerEntity(damageInfo.AttackerId).GameObject,
                    GameObject,
                    damageInfo
                ));
                Debug.LogWarning(Info.Name + "扣血" + finalDamage + "点！");
            }
        }

        private void Die(DamageInfo damageInfo)
        {
            //发布死亡事件
            EventManager.Instance.Publish(new CombatEventData(
                CombatEventData.EventType.Death,
                    EntityManager.Instance.GetPlayerEntity(damageInfo.AttackerId).GameObject,
                    GameObject,
                    damageInfo
            ));
            Debug.LogWarning(Info.Name + "死掉了");

            // 处理死亡逻辑
            //GameObject.SetActive(false);
        }

        private void HandleCombatEvent(CombatEventData eventData)
        {
            if (eventData.Target != GameObject) return;

            switch (eventData.Type)
            {
                case CombatEventData.EventType.DamageTaken:
                    PlayHitEffect(eventData.DamageInfo);
                    break;
                case CombatEventData.EventType.Death:
                    PlayDeathEffect();
                    break;
            }
        }

        private void PlayHitEffect(DamageInfo damageInfo)
        {
            // 实现受伤特效
            actions.Context.Animator.SetTrigger(AnimationConstant.Hit);
            RuntimeInfo.CombatStateTimer = RuntimeInfo.combatStateDuration;//受伤也要进入战斗状态
            RuntimeInfo.HitstunTimer = RuntimeInfo.hitStunDuration;
        }

        private void PlayDeathEffect()
        {
            // 实现死亡特效
            actions.Context.Animator.SetTrigger(AnimationConstant.Death);
            Info.IsDead = true;
        }

        int i = 0;
        public void Update()
        {
            if (RuntimeInfo.AttackTimer > 0)
            {
                i++;
                RuntimeInfo.AttackTimer -= Time.deltaTime;
                //Debug.Log($"{i},AttackTimer: {RuntimeInfo.AttackTimer}");
            }

            if (RuntimeInfo.AttackRecoveryTimer > 0)
            
                RuntimeInfo.AttackRecoveryTimer -= Time.deltaTime;
            



            if (RuntimeInfo.CombatStateTimer > 0)
                RuntimeInfo.CombatStateTimer -= Time.deltaTime;
            if (RuntimeInfo.HitstunTimer > 0)
                RuntimeInfo.HitstunTimer -= Time.deltaTime;
        }
    }
}