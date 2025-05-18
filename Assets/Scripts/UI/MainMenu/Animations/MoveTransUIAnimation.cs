using HFantasy.Script.UI.Base;
using UnityEngine;
using System.Collections;

namespace HFantasy.Script.UI.MainMenu.Animations
{
    public class MoveTransUIAnimation : UIAnimationBase
    {
        [System.Serializable]
        public class ButtonAnimationConfig
        {
            public RectTransform rectTransform;
            [Range(0, 360)] public float moveAngle = 0f; // 0度为向右，90度为向上
        }

        [SerializeField] private ButtonAnimationConfig[] buttonConfigs;
        [SerializeField] private float moveDistance = 1000f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve enterCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve exitCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector2[] originalPositions;
        private Coroutine[] currentAnimations;

        protected override void Awake()
        {
            base.Awake();
            originalPositions = new Vector2[buttonConfigs.Length];
            currentAnimations = new Coroutine[buttonConfigs.Length];

            for (int i = 0; i < buttonConfigs.Length; i++)
            {
                originalPositions[i] = buttonConfigs[i].rectTransform.anchoredPosition;
            }
        }

        public override void PlayEnterAnimation(float delay = 0f)
        {
            gameObject.SetActive(true);
            for (int i = 0; i < buttonConfigs.Length; i++)
            {
                if (currentAnimations[i] != null)
                    StopCoroutine(currentAnimations[i]);
                currentAnimations[i] = StartCoroutine(AnimatePosition(i, true, delay));
            }
        }

        public override void PlayExitAnimation(float delay = 0f)
        {
            for (int i = 0; i < buttonConfigs.Length; i++)
            {
                if (currentAnimations[i] != null)
                    StopCoroutine(currentAnimations[i]);
                currentAnimations[i] = StartCoroutine(AnimatePosition(i, false, delay));
            }
        }
        private Vector2 GetDirectionFromAngle(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
        private IEnumerator AnimatePosition(int index, bool isEnter, float delay)
        {
            isAnimating = true;

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            var config = buttonConfigs[index];
            float elapsed = 0;
            Vector2 moveDirection = GetDirectionFromAngle(config.moveAngle);

            Vector2 startPos = isEnter ?
                originalPositions[index] + moveDirection * moveDistance :
                originalPositions[index];
            Vector2 endPos = isEnter ?
                originalPositions[index] :
                originalPositions[index] + moveDirection * moveDistance;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float curveValue = isEnter ? enterCurve.Evaluate(t) : exitCurve.Evaluate(t);
                config.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);
                yield return null;
            }

            config.rectTransform.anchoredPosition = endPos;
            currentAnimations[index] = null;

            // 检查是否所有动画都完成
            bool allComplete = true;
            for (int i = 0; i < currentAnimations.Length; i++)
            {
                if (currentAnimations[i] != null)
                {
                    allComplete = false;
                    break;
                }
            }

            if (allComplete)
            {
                OnAnimationComplete(isEnter);
            }
        }
    }
}