using HFantasy.Script.Player.PlayerCamera.Follow;
using HFantasy.Script.Player.PlayerCamera.Person;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Core
{
    public class MainCameraController : MonoBehaviour
    {
        public enum CameraFollowMode
        {
            Center,
            Fixed25D,
        }

        public Transform target;
        private ICameraMode currentMode;
        private ICameraFollowMode currentFollow;
        private CameraFollowMode currentFollowMode = CameraFollowMode.Center;

        private readonly Dictionary<CameraFollowMode, ICameraFollowMode> followModes;

        public MainCameraController()
        {
            followModes = new Dictionary<CameraFollowMode, ICameraFollowMode>
            {
                { CameraFollowMode.Center, new CenterCameraFollowMode() },
                { CameraFollowMode.Fixed25D, new Fixed25DCameraFollowMode() }
            };
        }

        void Start()
        {
            SwitchCameraMode(CameraFollowMode.Center);
            InputManager.Instance.OnCameraModeChanged += OnCameraModeChanged;
        }

        void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F1)) SwitchCameraMode(CameraFollowMode.Center);
            if (Input.GetKeyDown(KeyCode.F2)) SwitchCameraMode(CameraFollowMode.Fixed25D);

            if (currentFollow != null && transform != null)
            {
                currentFollow.UpdateFollow(transform, target, InputManager.Instance.LookInput, InputManager.Instance.ZoomInput);
            }
            else Debug.LogWarning("相机跟随有空对象");
        }

        private void OnCameraModeChanged()
        {
            //循环切换
            currentFollowMode = (CameraFollowMode)(((int)currentFollowMode + 1) % System.Enum.GetValues(typeof(CameraFollowMode)).Length);
            SwitchCameraMode(currentFollowMode);
        }

        public void SwitchCameraMode(CameraFollowMode mode)
        {
            currentFollowMode = mode;
            currentFollow = followModes[mode];

            //设置光标状态
            switch (mode)
            {
                case CameraFollowMode.Center:
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                case CameraFollowMode.Fixed25D:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }

        void OnDestroy()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnCameraModeChanged -= OnCameraModeChanged;
        }
    }
}
