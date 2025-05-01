using UnityEngine;

namespace HFantasy.Script.Player.Camera.Follow
{
    public interface ICameraFollowMode
    {
        void UpdateFollow(Transform cam, Transform target, Vector2 viewInput, float zoomInput);
    }
}

