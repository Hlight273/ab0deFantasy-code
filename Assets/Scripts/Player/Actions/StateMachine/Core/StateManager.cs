using HFantasy.Script.Player.Actions.StateMachine.States;
using System;
using System.Collections.Generic;

namespace HFantasy.Script.Player.Actions.StateMachine.Core 
{
    public class StateManager
    {
        private Dictionary<Type, IState> states = new();
        private IState currentState;

        public void RegisterState<T>(T state) where T : IState
        {
            states[typeof(T)] = state;
        }

        public void TransitionTo<T>() where T : IState
        {
            if (!states.TryGetValue(typeof(T), out var newState)) return;
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void Update()
        {
            currentState?.Update();
            UnityEngine.Debug.LogWarning(currentState.GetType()); //´òÓ¡Íæ¼Ò×´Ì¬»ú×´Ì¬
        }
    } 
}