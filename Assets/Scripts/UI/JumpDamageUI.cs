using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
namespace HFantasy.Script.UI
{

    /// <summary>
    /// 伤害数字UI类
    /// </summary>
    public class JumpDamageUI : MonoBehaviour
    {
        [SerializeField] private GameObject numberPrefab;
        [Header("动画参数")]
        [SerializeField] private float jumpHeight = 50f;
        [SerializeField] private float jumpDuration = 0.5f;
        [SerializeField] private float stayDuration = 0.3f;
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private float heightOffset = 1.8f; // 目标头顶偏移
        [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private List<TextMeshProUGUI> numberTexts = new List<TextMeshProUGUI>();
        private Transform targetTransform;
        private Vector2 relativeOffset;
        private Camera mainCamera;
        private float currentTime;
        private bool isJumping = true;
        private bool isStaying = false;
        private bool isFading = false;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (targetTransform == null) return;

            // 计算目标在屏幕上的位置（考虑高度偏移）
            Vector3 targetWorldPos = targetTransform.position + Vector3.up * heightOffset;
            Vector2 targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

            // 基础位置随目标实时更新
            Vector2 basePos = targetScreenPos + relativeOffset;

            currentTime += Time.deltaTime;

            if (isJumping)
            {
                float progress = currentTime / jumpDuration;
                if (progress >= 1f)
                {
                    isJumping = false;
                    isStaying = true;
                    currentTime = 0;
                }
                else
                {
                    float yOffset = jumpCurve.Evaluate(progress) * jumpHeight;
                    transform.position = basePos + Vector2.up * yOffset;
                }
            }
            else if (isStaying)
            {
                transform.position = basePos;
                if (currentTime >= stayDuration)
                {
                    isStaying = false;
                    isFading = true;
                    currentTime = 0;
                }
            }
            else if (isFading)
            {
                transform.position = basePos;
                float progress = currentTime / fadeDuration;
                if (progress >= 1f)
                {
                    Destroy(gameObject);
                }
                else
                {
                    float alpha = fadeCurve.Evaluate(progress);
                    foreach (var text in numberTexts)
                    {
                        var color = text.color;
                        color.a = alpha;
                        text.color = color;
                    }
                }
            }
        }

        public void Setup(int damage, bool isCritical = false, Transform target = null)
        {
            targetTransform = target;
            if (targetTransform != null)
            {
                // 计算初始屏幕位置
                Vector3 targetWorldPos = targetTransform.position + Vector3.up * heightOffset;
                Vector2 targetScreenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

                // 根据屏幕位置决定水平偏移方向
                float screenCenterX = Screen.width * 0.5f;
                float xOffset = targetScreenPos.x < screenCenterX ? 60f : -30f;

                // 保存相对偏移量
                relativeOffset = new Vector2(xOffset, 0);
                transform.position = targetScreenPos + relativeOffset;
            }

            string damageStr = damage.ToString();
            for (int i = 0; i < damageStr.Length; i++)
            {
                var numberObj = Instantiate(numberPrefab, transform);
                var text = numberObj.GetComponent<TextMeshProUGUI>();
                int spriteIndex = int.Parse(damageStr[i].ToString());
                text.text = $"<sprite={spriteIndex}>";

                if (isCritical)
                {
                    text.color = Color.red;
                    text.fontSize *= 1.3f;
                }

                numberTexts.Add(text);
            }
        }
        public void ResetCurves()
        {
            jumpCurve = new AnimationCurve(
                new Keyframe(0, 0, 0, 2),     // 开始时快速上升
                new Keyframe(0.6f, 1, 0, 0),  // 达到最高点
                new Keyframe(1, 0, -2, 0)     // 快速下落
            );

            fadeCurve = new AnimationCurve(
                new Keyframe(0, 1, 0, 0),     // 开始时完全不透明
                new Keyframe(0.7f, 1, 0, 0),  // 保持不透明
                new Keyframe(1, 0, -1, 0)     // 快速淡出
            );
        }
    }
    [CustomEditor(typeof(JumpDamageUI))]
    public class JumpDamageUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            JumpDamageUI script = (JumpDamageUI)target;

            EditorGUILayout.Space();
            if (GUILayout.Button("重置动画曲线"))
            {
                script.ResetCurves();
                EditorUtility.SetDirty(script);
            }
        }
    }
}