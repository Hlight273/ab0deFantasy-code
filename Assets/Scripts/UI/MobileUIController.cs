using HFantasy.Script.Core;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

namespace HFantasy.Script.UI
{
    public class MobileUIController : MonoBehaviour
    {
        [SerializeField] private GameObject mobileControls;
        [SerializeField] private OnScreenStick moveStick;
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button attackButton;
        [SerializeField] private Button switchCameraButton;
        [SerializeField] private float runThreshold = 0.8f;

        void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            mobileControls.SetActive(true);

            moveStick.controlPath = "<Gamepad>/leftStick";

            jumpButton.onClick.AddListener(() => {
                if (InputManager.Instance != null)
                {
                    InputManager.Instance.OnMobileJumpPressed();
                }
            });

            attackButton.onClick.AddListener(() => {
                if (InputManager.Instance != null)
                {
                    InputManager.Instance.OnMobileAttackPressed();
                }
            });

             switchCameraButton.onClick.AddListener(() => {
                if (InputManager.Instance != null)
                {
                    InputManager.Instance.OnMobileCameraModeChanged();
                }
            });
#else
            mobileControls.SetActive(false);
#endif
        }

        private void Update()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (InputManager.Instance != null && moveStick != null)
            {
                float stickValue = InputManager.Instance.MoveInput.magnitude;
                InputManager.Instance.OnMobileRunInput(stickValue >= runThreshold);
            }
#endif
        }

        void OnDestroy()
        {
            if (jumpButton != null)
                jumpButton.onClick.RemoveAllListeners();
            if (attackButton != null)
                attackButton.onClick.RemoveAllListeners();
            if (moveStick != null)
                moveStick.controlPath = null;
            if (switchCameraButton != null)
                switchCameraButton.onClick.RemoveAllListeners();
        }
    }
}