namespace HFantasy.Script.Player.StateMachine.States
{
    using HFantasy.Script.Entity.Player;
    using HFantasy.Script.Player.Movement;
    using HFantasy.Script.Player.StateMachine;

    public class WalkState : BasePlayerState
    {
        public WalkState(ICharactorMovement movement) : base(movement) { }

        public override void Enter()
        {
            movement.SetAnimation(PlayerStateType.Walk);
        }

        protected override void UpdateState()
        {
            movement.Move(movement.MoveInput);
        }

        public override void Exit() { }
    }
}