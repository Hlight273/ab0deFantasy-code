using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Follow
{
    public interface ICameraFollowMode
    {
        void UpdateFollow(Transform cam, Transform target, Vector2 viewInput, float zoomInput);
    }
}

