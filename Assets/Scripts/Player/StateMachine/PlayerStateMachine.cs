using System;
using System.Collections.Generic;
using System.Diagnostics;
using HFantasy.Script.Core;
using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class PlayerStateMachine
    {
        private readonly Dictionary<PlayerStateType, IPlayerState> states = new Dictionary<PlayerStateType, IPlayerState>();
        private readonly List<StateTransition> transitions = new List<StateTransition>();
        private readonly ICharactorMovement movement;

        private PlayerStateType currentStateType;

        public PlayerStateMachine(ICharactorMovement movement)
        {
            this.movement = movement;
            InitializeStates();
            InitializeTransitions();
        }

        private void InitializeStates()
        {
            states[PlayerStateType.Idle] = new IdleState(movement);
            states[PlayerStateType.Walk] = new WalkState(movement);
            states[PlayerStateType.Jump] = new JumpState(movement);
        }

        private void InitializeTransitions()
        {
            AddTransition(PlayerStateType.Idle, PlayerStateType.Walk,
                m => m.MoveInput.magnitude > 0.1f);

            AddTransition(PlayerStateType.Walk, PlayerStateType.Idle,
                m => m.MoveInput.magnitude < 0.1f);

            AddTransition(PlayerStateType.Idle, PlayerStateType.Jump,
                m => m.JumpPressed && m.IsGrounded); 

            AddTransition(PlayerStateType.Walk, PlayerStateType.Jump,
                m => m.JumpPressed && m.IsGrounded);

            AddTransition(PlayerStateType.Jump, PlayerStateType.Idle,
                m => m.IsGrounded && !m.IsJumpping && m.MoveInput.magnitude < 0.1f);

            AddTransition(PlayerStateType.Jump, PlayerStateType.Walk,
                m => m.IsGrounded && !m.IsJumpping && m.MoveInput.magnitude > 0.1f);
        }

        public void Update()
        {
            CheckTransitions();
            states[currentStateType].Update();
        }

        private void CheckTransitions()
        {
            //UnityEngine.Debug.LogWarning("m.JumpPressed" + movement.JumpPressed+ ",isGrounded"+movement.IsGrounded);
            foreach (var transition in transitions)
            {
                if (transition.FromState == currentStateType &&
                    transition.Condition(movement))
                {
                    ChangeState(transition.ToState);
                    break;
                }
            }
        }

        public void ChangeState(PlayerStateType newState)
        {
            states[currentStateType].Exit();
            currentStateType = newState;
            states[currentStateType].Enter();
        }

        private void AddTransition(PlayerStateType from, PlayerStateType to,
            Func<ICharactorMovement, bool> condition)
        {
            transitions.Add(new StateTransition(from, to, condition));
        }
    }
}
