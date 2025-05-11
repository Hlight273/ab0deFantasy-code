using HFantasy.Script.Core.CoreConfig;
using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Follow
{
    public class Fixed25DCameraFollowMode : ICameraFollowMode
    {
        private Quaternion fixedRotation;

        private Vector3 currentVelocity;


        public Fixed25DCameraFollowMode()
        {
            fixedRotation = GlobalCameraConfig.cameraFix25DdRotation;
        }

        public void UpdateFollow(Transform cam, Transform target, Vector2 viewInput, float zoomInput)
        {
            float dt = Mathf.Min(Time.deltaTime, 0.05f);
            cam.rotation = fixedRotation;
            UpdateCameraPosition(cam, target, dt);
        }

        /// <summary>
        /// ���㲢�������λ��
        /// </summary>
        private void UpdateCameraPosition(Transform cam, Transform target, float dt)
        {
            //λ����Ŀ��λ�� + ƫ��
            Vector3 focusPoint = target.position + Vector3.up * GlobalCameraConfig.cameraFix25DHeightOffset;
            focusPoint += -Vector3.forward * GlobalCameraConfig.cameraFix25DDistanceOffset;
            cam.position = Vector3.SmoothDamp(cam.position, focusPoint, ref currentVelocity, GlobalCameraConfig.positionSmoothTime);
        }
    }
}