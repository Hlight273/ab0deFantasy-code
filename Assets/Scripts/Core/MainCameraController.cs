using HFantasy.Script.Player.Camera.Follow;
using HFantasy.Script.Player.Camera.Person;
using UnityEngine;

namespace HFantasy.Script.Core
{
    public class MainCameraController : MonoBehaviour
    {
        public Transform target;

        private ICameraMode currentMode;
        private ICameraFollowMode currentFollow;

        private ThirdPersonCameraMode tpsMode = new ThirdPersonCameraMode();
        private FirstPersonCameraMode fpsMode = new FirstPersonCameraMode();

        private CenterCameraFollowMode centerFollow = new CenterCameraFollowMode();
        private Fixed25DCameraFollowMode fixed25DFollow = new Fixed25DCameraFollowMode();

        void Start()
        {
            SwitchToCenterFollowView();
        }

        void LateUpdate()
        {
            //if (Input.GetKeyDown(KeyCode.F1)) SwitchToFPS();
            //if (Input.GetKeyDown(KeyCode.F2)) SwitchToTPS();
            if (Input.GetKeyDown(KeyCode.F1)) SwitchToCenterFollowView();
            if (Input.GetKeyDown(KeyCode.F2)) SwitchTo25DView();

            //currentMode?.UpdateCamera(transform, target);
            currentFollow?.UpdateFollow(transform, target, InputManager.Instance.LookInput, InputManager.Instance.ZoomInput);
        }

        public void SwitchToTPS() => currentMode = tpsMode;
        public void SwitchToFPS() => currentMode = fpsMode;

        public void SwitchToCenterFollowView()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentFollow = centerFollow;
        }

        public void SwitchTo25DView()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            currentFollow = fixed25DFollow;
        }
    }
}
