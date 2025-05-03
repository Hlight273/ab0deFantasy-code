using System;
using System.Collections.Generic;
using System.Diagnostics;
using HFantasy.Script.Core;
using HFantasy.Script.Player.Combat;
using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class PlayerMovementStateMachine: IDomainStateMachine
    {
        private readonly Dictionary<PlayerMovementState, IPlayerState> states = new Dictionary<PlayerMovementState, IPlayerState>();
        private readonly List<StateTransition<PlayerMovementState, ICharactorMovement>> transitions = new();
        private readonly ICharactorMovement movement;

        private PlayerMovementState currentStateType;

        public PlayerMovementStateMachine(ICharactorMovement movement)
        {
            this.movement = movement;
            InitializeStates();
            InitializeTransitions();
        }

        private void InitializeStates()
        {
            states[PlayerMovementState.Idle] = new IdleState(movement);
            states[PlayerMovementState.Walk] = new WalkState(movement);
            states[PlayerMovementState.Jump] = new JumpState(movement);
            states[PlayerMovementState.Run] = new RunState(movement);
        }

        private void InitializeTransitions()
        {
            AddTransition(PlayerMovementState.Idle, PlayerMovementState.Walk,
                m => m.MoveInput.magnitude >= 0.1f);
            AddTransition(PlayerMovementState.Idle, PlayerMovementState.Jump,
               m => m.JumpPressed && m.IsGrounded);

            AddTransition(PlayerMovementState.Walk, PlayerMovementState.Idle,
                m => m.MoveInput.magnitude < 0.1f);
            AddTransition(PlayerMovementState.Walk, PlayerMovementState.Jump,
                m => m.JumpPressed && m.IsGrounded);
            AddTransition(PlayerMovementState.Walk, PlayerMovementState.Run,
                m => m.IsRunning);

            AddTransition(PlayerMovementState.Run, PlayerMovementState.Jump,
                m => m.JumpPressed && m.IsGrounded);
            AddTransition(PlayerMovementState.Run, PlayerMovementState.Walk,
                m => !m.IsRunning && m.MoveInput.magnitude >= 0.1f);
            AddTransition(PlayerMovementState.Run, PlayerMovementState.Walk,
                m => !m.IsRunning && m.MoveInput.magnitude < 0.1f);


            AddTransition(PlayerMovementState.Jump, PlayerMovementState.Idle,
                m => m.IsGrounded && !m.IsJumpping && m.MoveInput.magnitude < 0.1f);
            AddTransition(PlayerMovementState.Jump, PlayerMovementState.Walk,
                m => m.IsGrounded && !m.IsJumpping && m.MoveInput.magnitude >= 0.1f);


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

        public Enum GetCurrentState() => currentStateType;

        public void ChangeState(Enum newState)
        {
            states[currentStateType].Exit();
            currentStateType = (PlayerMovementState)newState;
            states[currentStateType].Enter();
        }

        private void AddTransition(PlayerMovementState from, PlayerMovementState to,
            Func<ICharactorMovement, bool> condition)
        {
            transitions.Add(new StateTransition<PlayerMovementState, ICharactorMovement>(from, to, condition));
        }
    }
}
