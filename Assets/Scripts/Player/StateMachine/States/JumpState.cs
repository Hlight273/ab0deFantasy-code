using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class JumpState : BasePlayerMoveState
    {

        public JumpState(ICharactorMovement movement):base(movement)
        {
        }

        public override void Enter()
        {
            movement.SetAnimation(PlayerMovementState.Jump);
            movement.Jump();
        }

        protected override void UpdateState()
        {
            movement.Jump();
            movement.Move(movement.MoveInput);
        }

        public override void Exit()
        {
            movement.ResetJump();
        }
    }
}
