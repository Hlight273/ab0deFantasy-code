using HFantasy.Script.Common;
using System;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HFantasy.Script.Core
{
    public class InputManager : MonoSingleton<InputManager>
    {
        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool RunPressed { get; private set; }
        public bool AttackPressed { get; private set; }

        public Vector2 LookInput { get; private set; }
        private InputAction lookAction;

        public float ZoomInput { get; private set; }
        private float lastPinchDistance = 0f;
        private float pinchZoom = 0f;
        private InputAction zoomAction;

        public event Action OnInteractPressed;

        private PlayerControls controls;

        

        protected override void Awake()
        {
            base.Awake();
            controls = new PlayerControls();

            lookAction = controls.Player.Look;
            zoomAction = controls.Player.Zoom;

            controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;

            controls.Player.Run.performed += ctx => RunPressed = true;
            controls.Player.Run.canceled += ctx => RunPressed = false;

            controls.Player.Jump.performed += _ => JumpPressed = true;

            controls.Player.Interact.performed += _ => OnInteractPressed?.Invoke();
            controls.Player.Attack.performed += ctx =>
            {
                if (!IsPointerOverUI())
                {
                    AttackPressed = true;
                }
            };

        }
        //这个加到以后的按钮onclick
        //public void OnAttackButtonClick()
        //{
        //    InputManager.Instance.OnAttackPressed?.Invoke();
        //}

        private void Update()
        {
            LookInput = lookAction.ReadValue<Vector2>();
            ZoomInput = GetZoom();
        }

        private void LateUpdate()
        {
            JumpPressed = false;
        }

        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();


        private float GetZoom()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return zoomAction.ReadValue<float>(); // 鼠标滚轮
#elif UNITY_IOS || UNITY_ANDROID
            return DetectPinch(); // 移动端
#else
            return 0f;
#endif
        }

        private float DetectPinch()
        {
            if (Input.touchCount != 2)
            {
                lastPinchDistance = 0f;
                return 0f;
            }

            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (touch0.phase == UnityEngine.TouchPhase.Ended || touch1.phase == UnityEngine.TouchPhase.Ended)
            {
                lastPinchDistance = 0f;
                return 0f;
            }

            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            if (lastPinchDistance == 0f)
            {
                lastPinchDistance = currentDistance;
                return 0f;
            }

            float delta = currentDistance - lastPinchDistance;
            lastPinchDistance = currentDistance;

            return delta * 0.01f; // 调整缩放敏感度
        }

        private bool IsPointerOverUI()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            return false;
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }

    }
}
