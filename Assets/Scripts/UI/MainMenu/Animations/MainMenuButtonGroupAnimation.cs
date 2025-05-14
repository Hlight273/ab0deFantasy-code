using HFantasy.Script.UI.Base;
using UnityEngine;
using System.Collections;

namespace HFantasy.Script.UI.MainMenu.Animations
{
    public class MainMenuButtonGroupAnimation : UIAnimationBase
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float moveDistance = 1000f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private bool moveLeft = true;
        [SerializeField] private AnimationCurve enterCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve exitCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector2 originalPosition;
        private Coroutine currentAnimation;

        protected override void Awake()
        {
            base.Awake();
            originalPosition = rectTransform.anchoredPosition;
        }

        public override void PlayEnterAnimation(float delay = 0f)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            gameObject.SetActive(true);
            currentAnimation = StartCoroutine(AnimatePosition(true, delay));
        }

        public override void PlayExitAnimation(float delay = 0f)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
                
            currentAnimation = StartCoroutine(AnimatePosition(false, delay));
        }

        private IEnumerator AnimatePosition(bool isEnter, float delay)
        {
            isAnimating = true;
            
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            float elapsed = 0;
            Vector2 startPos = isEnter ? 
                originalPosition + new Vector2(moveLeft ? -moveDistance : moveDistance, 0) : 
                originalPosition;
            Vector2 endPos = isEnter ? 
                originalPosition : 
                originalPosition + new Vector2(moveLeft ? -moveDistance : moveDistance, 0);

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                float curveValue = isEnter ? enterCurve.Evaluate(t) : exitCurve.Evaluate(t);
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);
                yield return null;
            }

            rectTransform.anchoredPosition = endPos;
            currentAnimation = null;
            OnAnimationComplete(isEnter);
        }

        protected override void OnAnimationComplete(bool isEnter)
        {
            base.OnAnimationComplete(isEnter);
            if(!isEnter)
                gameObject.SetActive(false);
        }
    }
}