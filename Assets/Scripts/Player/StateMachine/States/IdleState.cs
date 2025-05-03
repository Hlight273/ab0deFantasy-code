using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class IdleState : BasePlayerMoveState
    {

        public IdleState(ICharactorMovement movement) : base(movement)
        {
        }

        public override void Enter()
        {
            movement.SetAnimation(PlayerMovementState.Idle);
        }

        protected override void UpdateState()
        {
        }

        public override void Exit() { }
    }
}
