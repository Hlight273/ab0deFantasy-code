using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class WalkState : BasePlayerState
    {
        public WalkState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetBool(AnimationConstant.Walk, true);  // 改为Walk而不是Run
        }

        public override void Update()
        {
            Context.Movement.Move(Context.Movement.MoveInput);  // 移除乘以RunSpeedMultiplier

            if (Context.Movement.MoveInput.magnitude<0.1f)
                Context.StateManager.TransitionTo<IdleState>();
            else if (Context.Movement.IsRunning)
                Context.StateManager.TransitionTo<RunState>();
            else if (Context.Movement.JumpPressed)
                Context.StateManager.TransitionTo<JumpState>();
            else if (Context.Combat.CanAttack)
                Context.StateManager.TransitionTo<BoxingHitState>();
        }

        public override void Exit()
        {
            Context.Animator.SetBool(AnimationConstant.Walk, false);
        }
    }
}