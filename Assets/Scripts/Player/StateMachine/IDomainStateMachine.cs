namespace HFantasy.Script.Player.StateMachine
{
    using System;
    using System.Collections.Generic;

    public interface IDomainStateMachine
    {
        void Update();
        void ChangeState(Enum newState);
        Enum GetCurrentState();
    }
}