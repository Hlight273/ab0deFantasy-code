namespace HFantasy.Script.Player.StateMachine.States
{
    using HFantasy.Script.Player.Movement;
    using HFantasy.Script.Player.StateMachine;
    public abstract class BasePlayerMoveState : IPlayerState
    {
        protected readonly ICharactorMovement movement;

        protected BasePlayerMoveState(ICharactorMovement movement)
        {
            this.movement = movement;
        }

        public abstract void Enter();
        public abstract void Exit();

        public virtual void Update()
        {
            HandlePhysics();
            UpdateState();
        }

        protected abstract void UpdateState();

        private void HandlePhysics()
        {
            movement.HandlePhysics();
        }
    }
}