using UnityEngine;
using HFantasy.Script.Player.StateMachine;
using HFantasy.Script.Core.CoreConfig;
using HFantasy.Script.Player.Combat;

namespace HFantasy.Script.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig config;
        [SerializeField] private Transform groundCheck;

        private ICharactorMovement movementHandler;
        private ICharactorCombat combatHandler;
        private PlayerStateCoordinator coordinator;
        public bool IsLocalPlayer { get; set; }

        void Start()
        {
            var controller = GetComponent<CharacterController>();
            var animator = GetComponent<Animator>();
            var mainCamera = Camera.main.transform;

            movementHandler = new CharactorMovementHandler(
                controller,
                animator,
                config,
                groundCheck, mainCamera);
            combatHandler = new CharactorCombatHandler(controller,animator);

            coordinator = new PlayerStateCoordinator(movementHandler, combatHandler);
            coordinator.ChangeState(StateDomainType.Movement, PlayerMovementState.Idle);
        }

        void Update()
        {
            if (!IsLocalPlayer) return;
            coordinator.Update();
        }
    }
}