using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class IdleState : BasePlayerState
    {

        public IdleState(ICharactorMovement movement) : base(movement)
        {
        }

        public override void Enter()
        {
            movement.SetAnimation(PlayerStateType.Idle);
        }

        protected override void UpdateState()
        {
        }

        public override void Exit() { }
    }
}
