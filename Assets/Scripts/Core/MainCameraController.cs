using HFantasy.Script.Player.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Core
{
    public class MainCameraController : MonoBehaviour
    {
        public Transform target;
        private ICameraMode currentMode;

        private ThirdPersonCameraMode tpsMode = new ThirdPersonCameraMode();
        private FirstPersonCameraMode fpsMode = new FirstPersonCameraMode();

        void Start()
        {
            SwitchToTPS(); // ³õÊ¼ÎªTPS
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) SwitchToFPS();
            if (Input.GetKeyDown(KeyCode.F2)) SwitchToTPS();

            currentMode?.UpdateCamera(transform, target);
        }

        public void SwitchToTPS()
        {
            currentMode = tpsMode;
        }

        public void SwitchToFPS()
        {
            currentMode = fpsMode;
        }
    }
}
