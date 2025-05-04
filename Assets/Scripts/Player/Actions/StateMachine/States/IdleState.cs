using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class IdleState : BasePlayerState
    {
        public IdleState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
           // Context.Animator.SetBool(AnimationConstant.Idle, true);
        }

        public override void Update()
        {
            Context.Movement.HandlePhysics();
            if (Context.Movement.MoveInput.magnitude >= 0.1f)
                Context.StateManager.TransitionTo<WalkState>();
            else if (Context.Movement.JumpPressed)
                Context.StateManager.TransitionTo<JumpState>();
            else if (Context.Combat.AttackPressed)
                Context.StateManager.TransitionTo<BoxingHitState>();
        }

        public override void Exit()
        {
            //Context.Animator.SetBool(AnimationConstant.Idle, false);
        }
    }
}
