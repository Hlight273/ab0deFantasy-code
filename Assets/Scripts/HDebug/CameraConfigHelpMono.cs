using HFantasy.Script.Core.CoreConfig;
using UnityEngine;
namespace HFantasy.Script.HDebug
{
    public class CameraConfigHelpMono : MonoBehaviour
    {
        [Header("// Center ģʽ����")]
        public float mouseSensitivity;
        public float minPitch;
        public float maxPitch;

        public float rotationSmoothSpeed;
        public float positionSmoothTime;

        public float followDistance;
        public float minFollowDistance;
        public float maxFollowDistance;

        public float zoomSpeed;
        public float cameraHeightOffset;
        public float shoulderOffsetX;

        [Header("// 2.5D ģʽ����")]
        public float cameraFix25DHeightOffset;
        public float cameraFix25DDistanceOffset;
        public Vector3 cameraFix25DRotationEuler;

        [Header("���Կ���")]
        public bool applyEveryFrame = true; // �Ƿ�ÿ֡���²���

        void Start()
        {
            mouseSensitivity = GlobalCameraConfig.mouseSensitivity;
            minPitch = GlobalCameraConfig.minPitch;
            maxPitch = GlobalCameraConfig.maxPitch;

            rotationSmoothSpeed = GlobalCameraConfig.rotationSmoothSpeed;
            positionSmoothTime = GlobalCameraConfig.positionSmoothTime;

            followDistance = GlobalCameraConfig.followDistance;
            minFollowDistance = GlobalCameraConfig.minFollowDistance;
            maxFollowDistance = GlobalCameraConfig.maxFollowDistance;

            zoomSpeed = GlobalCameraConfig.zoomSpeed;
            cameraHeightOffset = GlobalCameraConfig.cameraHeightOffset;
            shoulderOffsetX = GlobalCameraConfig.shoulderOffsetX;

            cameraFix25DHeightOffset = GlobalCameraConfig.cameraFix25DHeightOffset;
            cameraFix25DDistanceOffset = GlobalCameraConfig.cameraFix25DDistanceOffset;
            cameraFix25DRotationEuler = GlobalCameraConfig.cameraFix25DdRotation.eulerAngles;
        }

        void Update()
        {
            if (!applyEveryFrame) return;

            GlobalCameraConfig.mouseSensitivity = mouseSensitivity;
            GlobalCameraConfig.minPitch = minPitch;
            GlobalCameraConfig.maxPitch = maxPitch;

            GlobalCameraConfig.rotationSmoothSpeed = rotationSmoothSpeed;
            GlobalCameraConfig.positionSmoothTime = positionSmoothTime;

            GlobalCameraConfig.followDistance = followDistance;
            GlobalCameraConfig.minFollowDistance = minFollowDistance;
            GlobalCameraConfig.maxFollowDistance = maxFollowDistance;

            GlobalCameraConfig.zoomSpeed = zoomSpeed;
            GlobalCameraConfig.cameraHeightOffset = cameraHeightOffset;
            GlobalCameraConfig.shoulderOffsetX = shoulderOffsetX;

            GlobalCameraConfig.cameraFix25DHeightOffset = cameraFix25DHeightOffset;
            GlobalCameraConfig.cameraFix25DDistanceOffset = cameraFix25DDistanceOffset;
            GlobalCameraConfig.cameraFix25DdRotation = Quaternion.Euler(cameraFix25DRotationEuler);
        }
    }
}