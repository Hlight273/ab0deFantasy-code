namespace HFantasy.Script.Player.StateMachine
{
    public interface IPlayerState
    {
        void Enter();
        void Exit();
        void Update();
        //void FixedUpdate();
        //void LateUpdate();
    }
}