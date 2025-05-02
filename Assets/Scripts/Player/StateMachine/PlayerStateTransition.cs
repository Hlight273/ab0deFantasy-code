using HFantasy.Script.Player.Movement;
using System;

namespace HFantasy.Script.Player.StateMachine
{
    public enum PlayerStateType
    {
        Idle,
        Walk,
        Jump
    }

    public class StateTransition
    {
        public PlayerStateType FromState { get; }
        public PlayerStateType ToState { get; }
        public Func<ICharactorMovement, bool> Condition { get; }

        public StateTransition(PlayerStateType from, PlayerStateType to, Func<ICharactorMovement, bool> condition)
        {
            FromState = from;
            ToState = to;
            Condition = condition;
        }
    }
}