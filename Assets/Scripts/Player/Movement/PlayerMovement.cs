using UnityEngine;
using HFantasy.Script.Player.StateMachine;
using HFantasy.Script.Core.CoreConfig;

namespace HFantasy.Script.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig config;
        [SerializeField] private Transform groundCheck;

        private ICharactorMovement movementHandler;
        private PlayerStateMachine stateMachine;
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

            stateMachine = new PlayerStateMachine(movementHandler);
            stateMachine.ChangeState(PlayerStateType.Idle);
        }

        void Update()
        {
            if (!IsLocalPlayer) return;
            stateMachine?.Update();
        }
    }
}