using HFantasy.Script.Core;
using HFantasy.Script.Core.CoreConfig;
using UnityEngine;

namespace HFantasy.Script.Player.PlayerCamera.Follow
{
    /// <summary>
    /// 角色居中视角相机跟随模式
    /// 支持视角旋转、缩放、碰撞避障等基础相机行为
    /// </summary>
    public class CenterCameraFollowMode : ICameraFollowMode
    {
        // 当前水平旋转角度
        private float yaw;

        // 当前垂直旋转角度（初始为10度，避免镜头正水平）
        private float pitch = 10f;

        // 当前相机旋转
        private Quaternion currentRotation = Quaternion.identity;

        // 相机位置平滑速度缓存（用于 SmoothDamp）
        private Vector3 currentVelocity;

        // 视角输入平滑处理缓存
        private Vector2 smoothLookInput;
        private Vector2 lookInputVelocity;

        // 当前相机与目标之间的距离
        private float currentFollowDistance;

        public CenterCameraFollowMode()
        {
            // 初始化跟随距离为全局配置值
            currentFollowDistance = GlobalCameraConfig.followDistance;
        }

        /// <summary>
        /// 相机更新逻辑，每帧调用
        /// </summary>
        /// <param name="cam">相机 Transform</param>
        /// <param name="target">目标 Transform</param>
        /// <param name="viewInput">视角输入（如鼠标或右摇杆）</param>
        /// <param name="zoomInput">缩放输入（如滚轮或捏合）</param>
        public void UpdateFollow(Transform cam, Transform target, Vector2 viewInput, float zoomInput)
        {
            // 限制最大帧间隔，避免跳帧引发镜头跳跃
            float dt = Mathf.Min(Time.deltaTime, 0.05f);

            HandleLook(viewInput, dt);              // 更新视角角度
            HandleRotation(cam, dt);                // 应用旋转
            HandleZoom(zoomInput, dt);              // 缩放相机距离
            UpdateCameraPosition(cam, target, dt);  // 更新相机位置
        }

        /// <summary>
        /// 获取当前相机距离（可用于 UI 显示）
        /// </summary>
        public float GetCurrentZoom() => currentFollowDistance;

        /// <summary>
        /// 处理用户视角输入（旋转）
        /// </summary>
        private void HandleLook(Vector2 viewInput, float dt)
        {
            // 平滑输入，避免跳跃
            smoothLookInput = Vector2.SmoothDamp(smoothLookInput, viewInput, ref lookInputVelocity, 0.01f);

            // 更新角度
            yaw += smoothLookInput.x * GlobalCameraConfig.mouseSensitivity;
            pitch -= smoothLookInput.y * GlobalCameraConfig.mouseSensitivity;

            // 限制俯仰角度范围
            pitch = Mathf.Clamp(pitch, GlobalCameraConfig.minPitch, GlobalCameraConfig.maxPitch);
        }

        /// <summary>
        /// 应用旋转至相机
        /// </summary>
        private void HandleRotation(Transform cam, float dt)
        {
            // 初始化旋转角（避免一开始 slerp 无意义）
            if (currentRotation == Quaternion.identity)
                currentRotation = Quaternion.Euler(pitch, yaw, 0f);

            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

            // 平滑插值旋转
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, dt * GlobalCameraConfig.rotationSmoothSpeed);
            cam.rotation = currentRotation;
        }

        /// <summary>
        /// 缩放镜头（更改相机距离）
        /// </summary>
        private void HandleZoom(float zoomInput, float dt)
        {
            // 输入过小则忽略，避免手指误触或滚轮抖动
            if (Mathf.Abs(zoomInput) < 0.001f) return;

            currentFollowDistance -= zoomInput * GlobalCameraConfig.zoomSpeed * dt;

            // 限制最小最大距离
            currentFollowDistance = Mathf.Clamp(currentFollowDistance, GlobalCameraConfig.minFollowDistance, GlobalCameraConfig.maxFollowDistance);
        }

        /// <summary>
        /// 计算并更新相机位置
        /// </summary>
        private void UpdateCameraPosition(Transform cam, Transform target, float dt)
        {
            // 焦点位置：目标位置 + 偏移（如人物头顶）
            Vector3 focusPoint = target.position + Vector3.up * GlobalCameraConfig.cameraHeightOffset;

            // 根据旋转和距离计算目标相机位置
            Vector3 desiredCameraPos = focusPoint - currentRotation * Vector3.forward * currentFollowDistance;

            float smoothTime = GlobalCameraConfig.positionSmoothTime;

            // 处理碰撞（防止穿墙）
            RaycastHit hit;
            Vector3 direction = desiredCameraPos - focusPoint;

          

            if (Physics.Raycast(focusPoint, direction.normalized, out hit, currentFollowDistance, GlobalCameraConfig.collisionMask))
            {
                // 调整目标位置为碰撞点稍前一点，避免直接贴墙
                desiredCameraPos = focusPoint + direction.normalized * (hit.distance - 0.2f);
                if (direction.magnitude <= 6f)
                    smoothTime *= 5f; // 如果距离很短，增加平滑时间，避免抖动
            }

            // 平滑移动相机到目标位置
            cam.position = Vector3.SmoothDamp(cam.position, desiredCameraPos, ref currentVelocity, smoothTime);
        }
    }
}
