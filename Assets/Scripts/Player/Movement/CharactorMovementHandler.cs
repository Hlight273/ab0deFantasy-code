using UnityEngine;
using HFantasy.Script.Core;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.Player.StateMachine;
using HFantasy.Script.Core.CoreConfig;

namespace HFantasy.Script.Player.Movement
{
    public class CharactorMovementHandler : ICharactorMovement
    {
        private readonly UnityEngine.CharacterController controller;
        private readonly Animator animator;
        private readonly Transform groundCheck;
        private readonly Transform cam;
        private readonly LayerMask groundLayer;

        // 配置参数
        private readonly float moveSpeed;
        private readonly float jumpHeight;
        private readonly float jumpRange;
        private readonly float gravity;
        private readonly float groundDistance;
        private readonly float jumpStartOffset;
        private readonly float jumpEndOffset;
        private readonly float jumpEndTime;

        private Vector3 velocity;
        private bool isGrounded;
        private bool jumpRequested;
        private float jumpTimer;
        private bool hasJumped;

        public bool IsGrounded => isGrounded;
        public bool IsJumpping => jumpRequested;
        public Vector2 MoveInput => InputManager.Instance.MoveInput;

        public bool JumpPressed => InputManager.Instance.JumpPressed;

        public CharactorMovementHandler(
            UnityEngine.CharacterController controller,
            Animator animator,
            PlayerMovementConfig config,
            Transform groundCheck,
            Transform mainCamera)
        {
            this.controller = controller;
            this.animator = animator;
            this.groundCheck = groundCheck;
            this.cam = mainCamera;

            this.groundLayer = config.GroundLayer;
            this.groundDistance = config.GroundDistance;
            this.moveSpeed = config.MoveSpeed;
            this.gravity = config.Gravity;
            this.jumpHeight = config.JumpHeight;
            this.jumpRange = config.JumpRange;
            this.jumpStartOffset = config.JumpStartOffset;
            this.jumpEndOffset = config.JumpEndOffset;
            this.jumpEndTime = config.JumpEndTime;
        }

        public void Move(Vector2 input)
        {
            Vector3 moveDirection;

            if (jumpRequested)
            {
                if (jumpTimer <= jumpStartOffset || jumpTimer >= jumpEndOffset)
                {
                    input = Vector2.zero;
                }
                else
                {
                    input *= jumpRange;
                }
            }

            moveDirection = GetCameraRelativeDirection(input);

            if (moveDirection.magnitude > 0.01f)
            {
                HandleRotation(moveDirection);
            }

            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        public void Jump()
        {
            if (!jumpRequested && isGrounded && JumpPressed)
            {
                jumpRequested = true;
                jumpTimer = 0f;
                hasJumped = false;
            }

            if (jumpRequested)
            {
                jumpTimer += Time.deltaTime;

                if (!hasJumped && jumpTimer >= jumpStartOffset)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    hasJumped = true;
                }

                if (jumpTimer >= jumpEndTime)
                {
                    ResetJump();
                }
            }
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }

        private void HandleRotation(Vector3 direction)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            controller.transform.rotation = Quaternion.RotateTowards(
                controller.transform.rotation,
                targetRotation,
                720f * Time.deltaTime);
        }

        public void HandlePhysics()
        {
            CheckGroundStatus();
            ApplyGravity();
            ApplyVelocity();
        }

        private void CheckGroundStatus()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }

        private void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
        }

        private void ApplyVelocity()
        {
            controller.Move(velocity * Time.deltaTime);
        }

        public void SetAnimation(PlayerStateType state)
        {
            if (animator == null) return;

            // 先重置所有动画状态
            animator.SetBool(AnimationConstant.Walk, false);
            animator.SetBool(AnimationConstant.Jump, false);

            // 设置当前状态
            switch (state)
            {
                case PlayerStateType.Walk:
                    animator.SetBool(AnimationConstant.Walk, true);
                    break;
                case PlayerStateType.Jump:
                    animator.SetBool(AnimationConstant.Jump, true);
                    break;
                case PlayerStateType.Idle:

                    break;
            }
        }

        public void ResetJump()
        {
            jumpRequested = false;
            jumpTimer = 0;
        }
    }
}