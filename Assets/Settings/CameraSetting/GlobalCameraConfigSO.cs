using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "新建Scriptable配置/GlobalCameraConfig")]
public class GlobalCameraConfigSO : ScriptableObject
{
    [Header("Center 模式参数")]
    public float mouseSensitivity = 0.05f;
    public float minPitch = -30f;
    public float maxPitch = 90f;
    public float rotationSmoothSpeed = 40f;
    public float positionSmoothTime = 0.02f;
    public float followDistance = 5.5f;
    public float minFollowDistance = 2.5f;
    public float maxFollowDistance = 8.0f;
    public float zoomSpeed = 0.5f;
    public float cameraHeightOffset = 1.5f;
    public float shoulderOffsetX = 0.5f;

    [Header("2.5D 模式参数")]
    public float cameraFix25DHeightOffset = 4.5f;
    public float cameraFix25DDistanceOffset = 4.5f;
    public Quaternion cameraFix25DdRotation = Quaternion.Euler(45f, 0f, 0f);

    [Header("碰撞检测")]
    public LayerMask collisionMask = Physics.DefaultRaycastLayers;
}
