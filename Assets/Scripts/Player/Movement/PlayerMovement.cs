using HFantasy.Script.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HFantasy.Script.Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("�ƶ�����")]
        public float moveSpeed = 1.74f;

        [Header("��Ծ����")]
        public float jumpHeight = 0.2f;
        public float jumpRange = 2.48f;
        public float gravity = -9.81f;

        [Header("�����͵�����ƫ��ʱ��")]
        public float jumpStartOffset = 0.75f;//��ʼ����
        public float jumpEndOffset = 1.64f;//���ʱ��
        public float jumpEndTime = 1.85f;//ȫ�׶�������ʱ��

        private bool jumpRequested = false;
        private float jumpTimer = 0f;
        private bool hasJumped = false;

        [Header("������")]
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

        /// <summary>����ɫ�Ƿ��ڵ���</summary>
        void CheckGroundStatus()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // �ȶ�����
            }
        }

        /// <summary>�����ƶ��߼����������������</summary>
        void HandleMovement(Vector2 input)
        {
            // ��Ծ��������Ҫ�ر���
            if (IsJumpping)
            {
                if (jumpTimer <= jumpStartOffset || jumpTimer >= jumpEndOffset)//��������ص�ʱ�����ƶ�
                {
                    input = Vector2.zero;  // ��ֹˮƽ���������
                }
                else//���Ĺ�����Ҫ��ˮƽ����
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

        /// <summary>��ɫ�����ƶ�����</summary>
        void HandleRotation(Vector3 direction)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
        }

        /// <summary>������Ծ</summary>
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

                // ��Ծ������ʱ�䳬������ĩβ���ɫ���ڿ���
                //if (jumpTimer >= jumpEndOffset || (hasJumped && isGrounded ))
                if (jumpTimer >= jumpEndTime)
                {
                    jumpRequested = false;
                    jumpTimer = 0f;
                    hasJumped = false;
                }
            }
        }

        /// <summary>�����������ٶ�</summary>
        void ApplyGravity()
        {
            velocity.y += gravity * Time.deltaTime;
        }

        /// <summary>����ֱ�ٶ�Ӧ�õ���ɫ������</summary>
        void ApplyVelocity()
        {
            Controller.Move(velocity * Time.deltaTime);
        }

        /// <summary>��ȡ��������������ƶ�����</summary>
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