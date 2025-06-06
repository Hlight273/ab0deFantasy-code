using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class CombatState : BasePlayerState
    {
        public CombatState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetBool(AnimationConstant.Combat, true);
        }

        public override void Update()
        {
            if (Context.Combat.CanAttack)
                Context.StateManager.TransitionTo<BoxingHitState>();
            else if (!Context.Combat.IsCombatState)
                Context.StateManager.TransitionTo<IdleState>();
            else if (Context.Movement.MoveInput.magnitude>=0.1f)
                Context.StateManager.TransitionTo<WalkState>();
            else if (Context.Movement.IsRunning)
                Context.StateManager.TransitionTo<RunState>();

        }

        public override void Exit()
        {
            Context.Animator.SetBool(AnimationConstant.Combat, false);
        }
    }
}