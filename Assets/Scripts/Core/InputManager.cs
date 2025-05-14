using HFantasy.Script.Common;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HFantasy.Script.Core
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private bool isPointerOverUI;
        public Vector2 MoveInput { get; private set; }//移动输入值
        public bool JumpPressed { get; private set; }//是否跳跃
        public bool RunPressed { get; private set; }//是否奔跑
        public bool AttackPressed { get; private set; }//是否攻击


        public Vector2 LookInput { get; private set; }//视角滑动值
        private Vector2 lastTouchPosition;
        private bool isTouchingLook = false;

        private InputAction lookAction;
        public float ZoomInput { get; private set; }//视角缩放值

        private float lastPinchDistance = 0f;
        private InputAction zoomAction;

        public event Action OnInteractPressed;//按F交互
        private bool hasInteractableTarget;
        public bool HasInteractableTarget => hasInteractableTarget && !IsPointerOverUI();//当前是否指着可交互物体

        public event Action OnCameraModeChanged;//F1F2或者按钮切换相机视角

        private PlayerControls controls;

        private bool CanInput => !(EntityManager.Instance.MyPlayerEntity == null || EntityManager.Instance.MyPlayerEntity.RuntimeInfo.IsHitstun);

        protected override void Awake()
        {
            base.Awake();
            controls = new PlayerControls();

            lookAction = controls.Player.Look;
            zoomAction = controls.Player.Zoom;

            controls.Player.Move.performed += ctx => MoveInput = CanInput ? ctx.ReadValue<Vector2>() : Vector2.zero;
            controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;

            controls.Player.Run.performed += ctx => RunPressed = CanInput;
            controls.Player.Run.canceled += ctx => RunPressed = false;

            controls.Player.Jump.performed += _ => JumpPressed = CanInput;
            controls.Player.Interact.performed += _ => OnInteractPressed?.Invoke();

             controls.Player.SwitchCamera.performed += _ => OnCameraModeChanged?.Invoke();

            controls.Player.Attack.performed += ctx =>
            {
                if (!IsPointerOverUI() && CanInput)
                {
                    AttackPressed = true;
                }
            };
        }

        private void Update()
        {
#if UNITY_ANDROID || UNITY_IOS
        isPointerOverUI = CheckPointerOverUI();
        HandleMobileLookAndZoom();
#else
            isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
            LookInput = lookAction.ReadValue<Vector2>();
            ZoomInput = zoomAction.ReadValue<float>();
#endif
        }

        private void LateUpdate()
        {
            JumpPressed = false;
            AttackPressed = false;
        }

        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();

        private void HandleMobileLookAndZoom()
        {
            ZoomInput = 0f;
            LookInput = Vector2.zero;

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (!IsPointerOverUI(touch.fingerId) && touch.position.x > Screen.width / 2f)
                {
                    if (touch.phase == UnityEngine.TouchPhase.Began)
                    {
                        lastTouchPosition = touch.position;
                        isTouchingLook = true;
                    }
                    else if (touch.phase == UnityEngine.TouchPhase.Moved && isTouchingLook)
                    {
                        Vector2 delta = touch.deltaPosition;
                        LookInput = delta / Screen.dpi * 500f;
                    }
                }

                lastPinchDistance = 0f;
            }
            else if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                if (MoveInput == Vector2.zero)
                {
                    //没有移动操作，当作缩放
                    float currentDistance = Vector2.Distance(touch0.position, touch1.position);

                    if (lastPinchDistance == 0f)
                    {
                        lastPinchDistance = currentDistance;
                    }
                    else
                    {
                        float delta = currentDistance - lastPinchDistance;
                        ZoomInput = delta * 0.6f; //缩放灵敏度
                        lastPinchDistance = currentDistance;
                    }

                    isTouchingLook = false;
                }
                else
                {
                    //有移动操作不当作缩放，而是尝试将另一个手指用于视角滑动
                    Touch lookTouch = GetLookTouchFromTwo(touch0, touch1);
                    if (!IsPointerOverUI(lookTouch.fingerId) && lookTouch.position.x > Screen.width / 2f)
                    {
                        if (lookTouch.phase == UnityEngine.TouchPhase.Moved)
                        {
                            Vector2 delta = lookTouch.deltaPosition;
                            LookInput = delta / Screen.dpi * 500f;
                        }
                    }

                    lastPinchDistance = 0f;
                }
            }
            else
            {
                lastPinchDistance = 0f;
                isTouchingLook = false;
            }
        }

        //哪个手指更可能是用于视角滑动的
        private Touch GetLookTouchFromTwo(Touch touch0, Touch touch1)
        {
            //默认选择右半屏的那个手指
            if (touch0.position.x > touch1.position.x)
                return touch0.position.x > Screen.width / 2f ? touch0 : touch1;
            else
                return touch1.position.x > Screen.width / 2f ? touch1 : touch0;
        }

        private bool CheckPointerOverUI()
        {
            if (Input.touchCount > 0)
            {
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
            return false;
        }
        private bool IsPointerOverUI(int touchId = -1)
        {
#if UNITY_ANDROID || UNITY_IOS
        if (touchId >= 0)
        {
            return EventSystem.current.IsPointerOverGameObject(touchId);
        }
        return isPointerOverUI;
#else
            return isPointerOverUI;
#endif
        }

        public void SetInteractableTarget(bool hasTarget)
        {
            hasInteractableTarget = hasTarget;
        }

        public void OnMobileJumpPressed()
        {
            if (CanInput)
            {
                JumpPressed = true;
            }
        }

        public void OnMobileAttackPressed()
        {
            if (!IsPointerOverUI() && CanInput)
            {
                AttackPressed = true;
            }
        }

        public void OnMobileRunInput(bool isRunning)
        {
            if (CanInput)
            {
                RunPressed = isRunning;
            }
        }

        public void OnMobileCameraModeChanged()
        {
            if (CanInput)
            {
                OnCameraModeChanged?.Invoke();
            }
        }
    }
}
