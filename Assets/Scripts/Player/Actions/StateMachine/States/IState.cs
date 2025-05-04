
namespace HFantasy.Script.Player.Actions.StateMachine.States
{
    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
        bool CanTransitionTo(IState nextState);
    }
}