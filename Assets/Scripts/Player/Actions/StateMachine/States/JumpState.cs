using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public class JumpState : BasePlayerState
    {
        public JumpState(PlayerContext context) : base(context) { }

        public override void Enter()
        {
            Context.Animator.SetBool(AnimationConstant.Jump, true);
           Context.Movement.Jump();
        }

        public override void Update()
        {
            Context.Movement.Move(Context.Movement.MoveInput);
            Context.Movement.Jump();
             Context.Movement.HandlePhysics();

            if (Context.Movement.IsGrounded && !Context.Movement.IsJumping)
            {
                if (Context.Movement.MoveInput.magnitude >= 0.1f)
                    Context.StateManager.TransitionTo<WalkState>();
                else
                    Context.StateManager.TransitionTo<IdleState>();
            }
        }

        public override void Exit()
        {
            Context.Animator.SetBool(AnimationConstant.Jump, false);
            Context.Movement.ResetJump();
        }
    }
}