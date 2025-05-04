
using HFantasy.Script.Player.Actions.Combat;
using HFantasy.Script.Player.Actions.Movement;
using UnityEngine;
namespace HFantasy.Script.Player.Actions.StateMachine.Core
{
    public class PlayerContext
    {
        public ICharactorMovement Movement { get; }
        public ICharactorCombat Combat { get; }
        public Animator Animator { get; }
        public StateManager StateManager { get; }

        public PlayerContext(ICharactorMovement movement, ICharactorCombat combat,
            Animator animator, StateManager stateManager)
        {
            Movement = movement;
            Combat = combat;
            Animator = animator;
            StateManager = stateManager;
        }
    }
}