using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class BoxingHitState : BasePlayerState
    {
        public BoxingHitState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetTrigger(AnimationConstant.BoxingHit);
            Context.Combat.PerformAttack();
        }

        public override void Update()
        {
            if (!Context.Combat.IsAttacking)
                Context.StateManager.TransitionTo<CombatState>();


        }

        public override void Exit()
        {
            //Context.Animator.SetBool(AnimationConstant.BoxingHit, false);
        }
    }
}