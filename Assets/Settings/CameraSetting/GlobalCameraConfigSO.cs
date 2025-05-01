using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "�½�Scriptable����/GlobalCameraConfig")]
public class GlobalCameraConfigSO : ScriptableObject
{
    [Header("Center ģʽ����")]
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

    [Header("2.5D ģʽ����")]
    public float cameraFix25DHeightOffset = 4.5f;
    public float cameraFix25DDistanceOffset = 4.5f;
    public Quaternion cameraFix25DdRotation = Quaternion.Euler(45f, 0f, 0f);

    [Header("��ײ���")]
    public LayerMask collisionMask = Physics.DefaultRaycastLayers;
}
