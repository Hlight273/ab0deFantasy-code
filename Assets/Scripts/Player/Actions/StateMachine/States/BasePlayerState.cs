using HFantasy.Script.Player.Actions.StateMachine.Core;

namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public abstract class BasePlayerState : IState
    {
        protected readonly PlayerContext Context;

        protected BasePlayerState(PlayerContext context)
        {
            Context = context;
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();

        public virtual bool CanTransitionTo(IState nextState) => true;
    }
}