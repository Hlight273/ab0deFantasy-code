using HFantasy.Script.Core;
using HFantasy.Script.Core.CoreConfig;
using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Follow
{
    /// <summary>
    /// ��ɫ�����ӽ��������ģʽ
    /// ֧���ӽ���ת�����š���ײ���ϵȻ��������Ϊ
    /// </summary>
    public class CenterCameraFollowMode : ICameraFollowMode
    {
        // ��ǰˮƽ��ת�Ƕ�
        private float yaw;

        // ��ǰ��ֱ��ת�Ƕȣ���ʼΪ10�ȣ����⾵ͷ��ˮƽ��
        private float pitch = 10f;

        // ��ǰ�����ת
        private Quaternion currentRotation = Quaternion.identity;

        // ���λ��ƽ���ٶȻ��棨���� SmoothDamp��
        private Vector3 currentVelocity;

        // �ӽ�����ƽ��������
        private Vector2 smoothLookInput;
        private Vector2 lookInputVelocity;

        // ��ǰ�����Ŀ��֮��ľ���
        private float currentFollowDistance;

        public CenterCameraFollowMode()
        {
            // ��ʼ���������Ϊȫ������ֵ
            currentFollowDistance = GlobalCameraConfig.followDistance;
        }

        /// <summary>
        /// ��������߼���ÿ֡����
        /// </summary>
        /// <param name="cam">��� Transform</param>
        /// <param name="target">Ŀ�� Transform</param>
        /// <param name="viewInput">�ӽ����루��������ҡ�ˣ�</param>
        /// <param name="zoomInput">�������루����ֻ���ϣ�</param>
        public void UpdateFollow(Transform cam, Transform target, Vector2 viewInput, float zoomInput)
        {
            // �������֡�����������֡������ͷ��Ծ
            float dt = Mathf.Min(Time.deltaTime, 0.05f);

            HandleLook(viewInput, dt);              // �����ӽǽǶ�
            HandleRotation(cam, dt);                // Ӧ����ת
            HandleZoom(zoomInput, dt);              // �����������
            UpdateCameraPosition(cam, target, dt);  // �������λ��
        }

        /// <summary>
        /// ��ȡ��ǰ������루������ UI ��ʾ��
        /// </summary>
        public float GetCurrentZoom() => currentFollowDistance;

        /// <summary>
        /// �����û��ӽ����루��ת��
        /// </summary>
        private void HandleLook(Vector2 viewInput, float dt)
        {
            // ƽ�����룬������Ծ
            smoothLookInput = Vector2.SmoothDamp(smoothLookInput, viewInput, ref lookInputVelocity, 0.01f);

            // ���½Ƕ�
            yaw += smoothLookInput.x * GlobalCameraConfig.mouseSensitivity;
            pitch -= smoothLookInput.y * GlobalCameraConfig.mouseSensitivity;

            // ���Ƹ����Ƕȷ�Χ
            pitch = Mathf.Clamp(pitch, GlobalCameraConfig.minPitch, GlobalCameraConfig.maxPitch);
        }

        /// <summary>
        /// Ӧ����ת�����
        /// </summary>
        private void HandleRotation(Transform cam, float dt)
        {
            // ��ʼ����ת�ǣ�����һ��ʼ slerp �����壩
            if (currentRotation == Quaternion.identity)
                currentRotation = Quaternion.Euler(pitch, yaw, 0f);

            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

            // ƽ����ֵ��ת
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, dt * GlobalCameraConfig.rotationSmoothSpeed);
            cam.rotation = currentRotation;
        }

        /// <summary>
        /// ���ž�ͷ������������룩
        /// </summary>
        private void HandleZoom(float zoomInput, float dt)
        {
            // �����С����ԣ�������ָ�󴥻���ֶ���
            if (Mathf.Abs(zoomInput) < 0.001f) return;

            currentFollowDistance -= zoomInput * GlobalCameraConfig.zoomSpeed * dt;

            // ������С������
            currentFollowDistance = Mathf.Clamp(currentFollowDistance, GlobalCameraConfig.minFollowDistance, GlobalCameraConfig.maxFollowDistance);
        }

        /// <summary>
        /// ���㲢�������λ��
        /// </summary>
        private void UpdateCameraPosition(Transform cam, Transform target, float dt)
        {
            // ����λ�ã�Ŀ��λ�� + ƫ�ƣ�������ͷ����
            Vector3 focusPoint = target.position + Vector3.up * GlobalCameraConfig.cameraHeightOffset;

            // ������ת�;������Ŀ�����λ��
            Vector3 desiredCameraPos = focusPoint - currentRotation * Vector3.forward * currentFollowDistance;

            float smoothTime = GlobalCameraConfig.positionSmoothTime;

            // ������ײ����ֹ��ǽ��
            RaycastHit hit;
            Vector3 direction = desiredCameraPos - focusPoint;

          

            if (Physics.Raycast(focusPoint, direction.normalized, out hit, currentFollowDistance, GlobalCameraConfig.collisionMask))
            {
                // ����Ŀ��λ��Ϊ��ײ����ǰһ�㣬����ֱ����ǽ
                desiredCameraPos = focusPoint + direction.normalized * (hit.distance - 0.2f);
                if (direction.magnitude <= 6f)
                    smoothTime *= 5f; // �������̣ܶ�����ƽ��ʱ�䣬���ⶶ��
            }

            // ƽ���ƶ������Ŀ��λ��
            cam.position = Vector3.SmoothDamp(cam.position, desiredCameraPos, ref currentVelocity, smoothTime);
        }
    }
}
