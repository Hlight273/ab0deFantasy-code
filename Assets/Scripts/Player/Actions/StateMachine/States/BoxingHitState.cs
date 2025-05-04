using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;
using UnityEngine;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class BoxingHitState : BasePlayerState
    {
        public BoxingHitState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetTrigger(AnimationConstant.BoxingHit);
            Context.Combat.PerformAttack();
            //Debug.LogError("BoxingHitState");
        }

        public override void Update()
        {
            Context.Animator.speed = Context.PlayerEntity.RuntimeInfo.BattleInfo.GetAnimationSpeedMultiplier();
            if (!Context.Combat.IsAttacking)
                Context.StateManager.TransitionTo<CombatState>();
            else if (!Context.Combat.IsAttackRecovering)//如果不是后摇，允许切换其他状态
            {
                if (!Context.Combat.IsCombatState)
                    Context.StateManager.TransitionTo<IdleState>();
                else if (Context.Movement.MoveInput.magnitude >= 0.1f)
                    Context.StateManager.TransitionTo<WalkState>();
                else if (Context.Movement.IsRunning)
                    Context.StateManager.TransitionTo<RunState>();
            }



        }

        public override void Exit()
        {
            Context.Animator.speed = 1f;
            //Context.Animator.SetBool(AnimationConstant.BoxingHit, false);
        }
    }
}