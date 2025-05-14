using UnityEngine;
using DG.Tweening;

namespace HFantasy.Script.UI
{
    /// <summary>
    /// ³éÌë²Ëµ¥¿ØÖÆÆ÷
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]

    public class DrawerMenu : MonoBehaviour
    {
        [Header("ÉèÖÃ")]
        [SerializeField] private RectTransform drawerPanel;
        [SerializeField] private float openPosition = 0f;
        [SerializeField] private float closedPosition = -300f;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private Ease easeType = Ease.OutQuad;

        private bool isOpen = false;
        private Tweener currentTween;

        public void ToggleDrawer()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
                currentTween = null;
            }

            float targetPosition = isOpen ? closedPosition : openPosition;
            Vector2 targetAnchorPos = new Vector2(targetPosition, drawerPanel.anchoredPosition.y);

            currentTween = DOTween.To(
                () => drawerPanel.anchoredPosition,
                x => drawerPanel.anchoredPosition = x,
                targetAnchorPos,
                duration
            ).SetEase(easeType)
             .OnComplete(() => {
                 isOpen = !isOpen;
                 currentTween = null;
             });
        }
    }
}