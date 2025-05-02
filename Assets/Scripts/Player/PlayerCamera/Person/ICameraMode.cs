using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Person
{
    public interface ICameraMode
    {
        void UpdateCamera(Transform cameraTransform, Transform target);
    }
}
