using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.Camera.Person
{
    public interface ICameraMode
    {
        void UpdateCamera(Transform cameraTransform, Transform target);
    }
}
