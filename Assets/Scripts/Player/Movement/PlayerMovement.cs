using HFantasy.Script.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HFantasy.Script.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("移动参数")]
        public float moveSpeed = 1.74f;

        [Header("跳跃参数")]
        public float jumpHeight = 0.2f;
        public float jumpRange = 2.48f;
        public float gravity = -9.81f;

        [Header("起跳和到地面偏移时间")]
        public float jumpStartOffset = 0.75f;//开始浮空
        public float jumpEndOffset = 1.64f;//落地时刻
        public float jumpEndTime = 1.85f;//全套动作结束时间

        private bool jumpRequested = false;
        private float jumpTimer = 0f;
        private bool hasJumped = false;

        [Header("地面检测")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundDistance = 0.4f;

        private CharacterController controller;
        private Transform cam;
        private Vector3 velocity;
        private bool isGrounded;

        public CharacterController Controller { get => controller; }
        public bool IsJumpping { get => jumpRequested; }
        public bool IsWalking;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            cam = UnityEngine.Camera.main.transform;
        }

        void Update()
        {
            Vector2 moveInput = InputManager.Instance.MoveInput;
            bool jumpPressed = InputManager.Instance.JumpPressed;


            CheckGroundStatus();
            ApplyGravity();
            HandleMovement(moveInput);
            HandleJump(jumpPressed);
            ApplyVelocity();
        }

        /// <summary>检测角色是否在地面</summary>
        void CheckGroundStatus()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // 稳定贴地
            }
        }

        /// <summary>处理移动逻辑（基于摄像机方向）</summary>
        void HandleMovement(Vector2 input)
        {
            // 跳跃过程中需要特别处理
            if (IsJumpping)
            {
                if (jumpTimer <= jumpStartOffset || jumpTimer >= jumpEndOffset)//起跳和落地的时候不让移动
                {
                    input = Vector2.zero;  // 禁止水平方向的输入
                }
                else//跳的过程中要加水平分量
                {
                    input *= jumpRange;
                }
            }
            

            Vector3 moveDirection = GetCameraRelativeDirection(input);

            if (moveDirection.magnitude > 0.01f)
            {
                IsWalking = true;
                HandleRotation(moveDirection);
            }
            else
            {
                IsWalking = false;
            }

            Controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        /// <summary>角色朝向移动方向</summary>
        void HandleRotation(Vector3 direction)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        }

        /// <summary>尝试跳跃</summary>
        void HandleJump(bool jumpPressed)
        {
            if (jumpPressed && isGrounded && !IsJumpping)
            {
                jumpRequested = true;
                jumpTimer = 0f;
                hasJumped = false;

            }

            if (IsJumpping)
            {
                jumpTimer += Time.deltaTime;

                if (!hasJumped && jumpTimer >= jumpStartOffset)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    hasJumped = true;
                }

                // 跳跃结束，时间超出动作末尾或角色不在空中
                //if (jumpTimer >= jumpEndOffset || (hasJumped && isGrounded ))
                if (jumpTimer >= jumpEndTime)
                {
                    jumpRequested = false;
                    jumpTimer = 0f;
                    hasJumped = false;
                }
            }
        }

        /// <summary>处理重力加速度</summary>
        void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
        }

        /// <summary>将竖直速度应用到角色控制器</summary>
        void ApplyVelocity()
        {
            Controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>获取相对摄像机方向的移动向量</summary>
        Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return forward * input.y + right * input.x;
        }
    }
}