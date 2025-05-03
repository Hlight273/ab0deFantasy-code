namespace HFantasy.Script.Player.StateMachine.States
{
    using HFantasy.Script.Entity.Player;
    using HFantasy.Script.Player.Movement;
    using HFantasy.Script.Player.StateMachine;

    public class RunState : BasePlayerMoveState
    {
        public RunState(ICharactorMovement movement) : base(movement) { }

        public override void Enter()
        {
            movement.SetAnimation(PlayerMovementState.Run);
        }

        protected override void UpdateState()
        {
            movement.Run(movement.MoveInput);
        }

        public override void Exit() { }
    }
}