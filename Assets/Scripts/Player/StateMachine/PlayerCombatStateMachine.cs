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
            currentState = PlayerCombatState.Boxing; // ��ʼ״̬
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
                c => !c.IsAttacking); // �������¹�����

            AddTransition(PlayerCombatState.BoxingHit, PlayerCombatState.Boxing,
                c => c.IsAttacking); // ������������������

            //AddTransition(PlayerCombatState.Boxing, PlayerCombatState.Hit,
            //    c => c.IsBeHitted); // ����������

            //AddTransition(PlayerCombatState.BoxingHit, PlayerCombatState.Hit,
            //    c => c.IsBeHitted); // ���������б����

            //AddTransition(PlayerCombatState.Hit, PlayerCombatState.Boxing,
            //    c => !c.IsBeHitted); // �ָ�����
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
