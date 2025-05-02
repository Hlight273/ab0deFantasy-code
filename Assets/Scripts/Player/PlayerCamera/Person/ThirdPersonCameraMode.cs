using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Person
{
    public class ThirdPersonCameraMode : ICameraMode
    {
        public Vector3 initOffset = new Vector3(0, 3, -5);
        public float followSpeed = 7f;

        public void UpdateCamera(Transform cam, Transform target)
        {
            Vector3 desired = target.position + initOffset;
            cam.position = Vector3.Lerp(cam.position, desired, followSpeed * Time.deltaTime);
            cam.LookAt(target);
        }
    }
}
