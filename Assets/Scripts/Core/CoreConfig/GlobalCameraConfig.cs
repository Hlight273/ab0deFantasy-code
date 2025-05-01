using UnityEngine;

namespace HFantasy.Script.Core.CoreConfig
{
    public static class GlobalCameraConfig
    {
        //center用的参数
        public static float mouseSensitivity = 0.05f;
        public static float minPitch = -30f;
        public static float maxPitch = 90f;

        public static float rotationSmoothSpeed = 40f;//平滑插值旋转参数
        public static float positionSmoothTime = 0.02f;//平滑插值移动参数

        public static float followDistance = 5.5f;
        public static float minFollowDistance = 2.5f; //最小缩放距离
        public static float maxFollowDistance = 8.0f; //最大缩放距离

        public static float zoomSpeed = 0.5f;

        public static float cameraHeightOffset = 1.5f;
        
        public static float shoulderOffsetX = 0.5f;

        //2.5D用的参数
        public static float cameraFix25DHeightOffset = 5.5f;
        public static float cameraFix25DDistanceOffset = 4.2f;
        public static Quaternion cameraFix25DdRotation = Quaternion.Euler(45f, 0f, 0f);

        public static LayerMask collisionMask = Physics.DefaultRaycastLayers; // 可指定为“CameraCollision”层
    }
}
