using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.Camera
{
    public class ThirdPersonCameraMode : ICameraMode
    {
        public Vector3 offset = new Vector3(0, 3, -6);
        public float followSpeed = 10f;

        public void UpdateCamera(Transform cam, Transform target)
        {
            Vector3 desired = target.position + offset;
            cam.position = Vector3.Lerp(cam.position, desired, followSpeed * Time.deltaTime);
            cam.LookAt(target);
        }
    }
}
