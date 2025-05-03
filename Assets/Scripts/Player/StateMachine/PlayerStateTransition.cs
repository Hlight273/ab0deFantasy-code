using HFantasy.Script.Player.Movement;
using System;

namespace HFantasy.Script.Player.StateMachine
{
 

    public class StateTransition<T,K>
    {
        public T FromState { get; }
        public T ToState { get; }
        public Func<K, bool> Condition { get; }

        public StateTransition(T from, T to, Func<K, bool> condition)
        {
            FromState = from;
            ToState = to;
            Condition = condition;
        }
    }
}