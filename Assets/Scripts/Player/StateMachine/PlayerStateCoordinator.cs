namespace HFantasy.Script.Player.StateMachine
{
    using HFantasy.Script.Player.Combat;
    using HFantasy.Script.Player.Movement;
    using HFantasy.Script.Player.StateMachine.States;
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    public class PlayerStateCoordinator : MonoBehaviour
    {
        private readonly Dictionary<StateDomainType, IDomainStateMachine> domains = new();

        public PlayerStateCoordinator(ICharactorMovement movement, ICharactorCombat combat)
        {
            domains[StateDomainType.Movement] = new PlayerMovementStateMachine(movement);
            domains[StateDomainType.Combat] = new PlayerCombatStateMachine(combat);
        }

        public void Update()
        {
            foreach (var domain in domains.Values)
                domain.Update();

            ResolveConflicts();
        }

        private void ResolveConflicts()
        {
            var move = (PlayerMovementState)domains[StateDomainType.Movement].GetCurrentState();
            var combat = (PlayerCombatState)domains[StateDomainType.Combat].GetCurrentState();

            //// Ê¾Àý³åÍ»´¦ÀíÂß¼­
            //if (combat == PlayerCombatState.BoxingHit || combat == PlayerCombatState.Hit)
            //{
            //    if (move == PlayerMovementState.Run)
            //    {
            //        domains[StateDomainType.Movement].ChangeState(PlayerMovementState.Idle);
            //    }
            //}
        }

        public void ChangeState(StateDomainType domain, Enum state)
        {
            domains[domain].ChangeState(state);
        }
    }
}