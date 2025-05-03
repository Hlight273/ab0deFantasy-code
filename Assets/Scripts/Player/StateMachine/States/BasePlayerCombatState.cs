namespace HFantasy.Script.Player.StateMachine.States
{
    using HFantasy.Script.Player.Combat;
    using HFantasy.Script.Player.Movement;
    using HFantasy.Script.Player.StateMachine;
    public abstract class BasePlayerCombatState : IPlayerState
    {
        protected readonly ICharactorCombat combat;

        protected BasePlayerCombatState(ICharactorCombat combat)
        {
            this.combat = combat;
        }

        public abstract void Enter();
        public abstract void Exit();

        public virtual void Update()
        {
            //HandlePhysics();
            UpdateState();
        }

        protected abstract void UpdateState();

        //private void HandlePhysics()
        //{
        //    movement.HandlePhysics();
        //}
    }
}