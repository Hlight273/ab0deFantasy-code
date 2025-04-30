using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.Camera
{
    public class FirstPersonCameraMode : ICameraMode
    {
        public Vector3 headOffset = new Vector3(0, 1.6f, 0);

        public void UpdateCamera(Transform cam, Transform target)
        {
            cam.position = target.position + headOffset;
            cam.rotation = target.rotation;
        }
    }
}
