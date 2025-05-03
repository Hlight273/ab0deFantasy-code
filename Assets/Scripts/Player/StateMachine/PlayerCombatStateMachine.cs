using HFantasy.Script.Player.Combat;
using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;
using System;
using System.Collections.Generic;

namespace HFantasy.Script.Player.StateMachine
{
    public class PlayerCombatStateMachine : IDomainStateMachine
    {
        private readonly Dictionary<PlayerCombatState, IPlayerState> states = new();
        private readonly List<StateTransition<PlayerCombatState, ICharactorCombat>> transitions = new();
        private readonly ICharactorCombat combat;

        private PlayerCombatState currentState;

        public PlayerCombatStateMachine(ICharactorCombat combat)
        {
            this.combat = combat;
            InitializeStates();
            InitializeTransitions();
            currentState = PlayerCombatState.Boxing; // 初始状态
            states[currentState].Enter();
        }

        private void InitializeStates()
        {
            states[PlayerCombatState.Boxing] = new BoxingState(combat);
            states[PlayerCombatState.BoxingHit] = new BoxingHitState(combat);
           // states[PlayerCombatState.Hit] = new HitState(combat);
        }

        private void InitializeTransitions()
        {
            AddTransition(PlayerCombatState.Boxing, PlayerCombatState.BoxingHit,
                c => !c.IsAttacking); // 例：按下攻击键

            AddTransition(PlayerCombatState.BoxingHit, PlayerCombatState.Boxing,
                c => c.IsAttacking); // 例：攻击动作播放完

            //AddTransition(PlayerCombatState.Boxing, PlayerCombatState.Hit,
            //    c => c.IsBeHitted); // 例：被击中

            //AddTransition(PlayerCombatState.BoxingHit, PlayerCombatState.Hit,
            //    c => c.IsBeHitted); // 例：攻击中被打断

            //AddTransition(PlayerCombatState.Hit, PlayerCombatState.Boxing,
            //    c => !c.IsBeHitted); // 恢复正常
        }

        public void Update()
        {
            CheckTransitions();
            states[currentState].Update();
        }

        private void CheckTransitions()
        {
            foreach (var transition in transitions)
            {
                if (transition.FromState.Equals(currentState) &&
                    transition.Condition(combat))
                {
                    ChangeState(transition.ToState);
                    break;
                }
            }
        }

        public Enum GetCurrentState() => currentState;

        public void ChangeState(Enum newState)
        {
            if (currentState.Equals(newState)) return;

            states[currentState].Exit();
            currentState = (PlayerCombatState)newState;
            states[currentState].Enter();
        }

        private void AddTransition(PlayerCombatState from, PlayerCombatState to,
            Func<ICharactorCombat, bool> condition)
        {
            transitions.Add(new StateTransition<PlayerCombatState, ICharactorCombat>(from, to, condition));
        }
    }
}
