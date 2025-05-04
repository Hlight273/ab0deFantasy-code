using HFantasy.Script.BattleSystem.Damage;
using HFantasy.Script.BattleSystem.Events;
using HFantasy.Script.Common;
using HFantasy.Script.Core.Events;
using UnityEngine;
namespace HFantasy.Script.UI
{


    public class DamagePopupManager : MonoSingleton<DamagePopupManager>
    {
        [SerializeField] private Canvas damageCanvas;
        [SerializeField] private GameObject damagePopupPrefab;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            EventManager.Instance.Subscribe<CombatEventData>(HandleCombatEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.Unsubscribe<CombatEventData>(HandleCombatEvent);
        }

        private void HandleCombatEvent(CombatEventData eventData)
        {
            if (eventData.Type == CombatEventData.EventType.DamageTaken)
            {
                CreateDamagePopup(eventData.Target.transform, eventData.DamageInfo);
            }
        }

        private void CreateDamagePopup(Transform targetTrans, DamageInfo damageInfo)
        {
            // 获取目标的视觉高度点
            Vector3 heightOffset = new Vector3(0, 1.8f, 0); // 一个适合大多数人形角色的高度
            Vector3 worldPos = targetTrans.position + heightOffset;
            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);

            // 根据目标在屏幕中的位置决定跳字偏移
            float screenCenterX = Screen.width * 0.5f;
            Vector2 offset = new Vector2(
                screenPos.x < screenCenterX ? 50f : -50f,  // 在屏幕左边时向右偏移，反之
                30f  // 向上偏移固定值
            );
            screenPos += offset;

            var popup = Instantiate(damagePopupPrefab, screenPos, Quaternion.identity, damageCanvas.transform);
            var damageUI = popup.GetComponent<JumpDamageUI>();
            damageUI.Setup((int)damageInfo.FinalDamage, false, targetTrans);
        }
    }
}