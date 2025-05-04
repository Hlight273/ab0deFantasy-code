using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class RunState : BasePlayerState
    {
        public RunState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetBool(AnimationConstant.Run, true);
        }

        public override void Update()
        {
            Context.Movement.Move(Context.Movement.MoveInput * Context.Movement.RunSpeedMultiplier);
            
            if (!Context.Movement.IsRunning)
                Context.StateManager.TransitionTo<WalkState>();
            else if (Context.Movement.JumpPressed)
                Context.StateManager.TransitionTo<JumpState>();
            else if (Context.Combat.CanAttack)
                Context.StateManager.TransitionTo<BoxingHitState>();
        }

        public override void Exit()
        {
            Context.Animator.SetBool(AnimationConstant.Run, false);
        }
    }
}