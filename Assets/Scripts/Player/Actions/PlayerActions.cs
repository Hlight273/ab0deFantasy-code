using UnityEngine;

using HFantasy.Script.Core.CoreConfig;
using HFantasy.Script.Player.Actions.StateMachine.Core;
using HFantasy.Script.Player.Actions.Movement;
using HFantasy.Script.Player.Actions.Combat;
using HFantasy.Script.Player.Actions.StateMachine.States;
using HFantasy.Script.Entity.Player;
using UnityEngine.InputSystem.XR;
using System;
using HFantasy.Script.Common.Constant;

namespace HFantasy.Script.Player.Actions
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField] private PlayerMovementConfig config;
        [SerializeField] private Transform groundCheck;

        private StateManager stateManager;

        private PlayerContext context;
        public PlayerContext Context => context;

        private CharactorMovementHandler movement;
        private CharactorCombatHandler combat;

        private PlayerEntity playerEntity;
        public PlayerEntity PlayerEntity => playerEntity;


        void Start()
        {
        }

        public void InitializePlayerEntity(PlayerEntity entity)
        {
            if (playerEntity != null)
            {
                Debug.LogError("PlayerActions�Ѿ���ʼ����PlayerEntity");
                return;
            }
            var controller = GetComponent<CharacterController>();
            var animator = GetComponent<Animator>();
            Transform mainCameraTransform = null;
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            playerEntity = entity;
            movement = new CharactorMovementHandler(
               controller,
               animator,
               config,
               groundCheck,
               mainCameraTransform);
            combat = new CharactorCombatHandler(playerEntity, controller, animator);

            stateManager = new StateManager();
            context = new PlayerContext(movement, combat, animator, stateManager, playerEntity);

            InitializeStates();
        }

        private void InitializeStates()
        {
            stateManager.RegisterState(new IdleState(Context));
            stateManager.RegisterState(new WalkState(Context));
            stateManager.RegisterState(new RunState(Context));
            stateManager.RegisterState(new JumpState(Context));
            stateManager.RegisterState(new CombatState(Context));
            stateManager.RegisterState(new BoxingHitState(Context));
            
            stateManager.TransitionTo<IdleState>();
        }

        private void UpdateAnyStateAnimation()
        {
             Context.Animator.SetBool(AnimationConstant.Combat, combat.IsCombatState);
        }

        void Update()
        {
            UpdateAnyStateAnimation();
            if (playerEntity!=null && !playerEntity.isLocalPlayer) return;
            stateManager.Update();
        }
        public void OnAttackHitFrame()//���������¼�
        {
            combat?.OnAttackHitFrame();
        }




        private void OnDrawGizmos()
        {
            if (combat == null || combat.PlayerEntity == null) return;

            var controller = GetComponent<CharacterController>();
            if (controller == null) return;

            //���ù�����Χ����ɫ
            Gizmos.color = Color.red;

            //���㹥�����ĵ�
            Vector3 attackCenter = combat.AttackCenter;

            //���ƹ�����Χ����
            Gizmos.DrawWireSphere(attackCenter, combat.PlayerEntity.RuntimeInfo?.BattleInfo?.AttackRange ?? 1f);

            //���ƹ�������
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, attackCenter);
        }

    }
}