using HFantasy.Script.UI.Base;
using UnityEngine;
using System.Collections;

namespace HFantasy.Script.UI.MainMenu.Animations
{
    /// <summary>
    /// 对外提供PlayEnterAnimation、PlayExitAnimation的UI动画类
    /// 控制UI元素的移动动画，该类挂在在UI界面元素上，内部sectionConfigs中配置各个部分的移动方向
    /// </summary>
    public class MoveTransUIAnimation : UIAnimationBase
    {
        [System.Serializable]
        public class SectionAnimationConfig
        {
            public RectTransform rectTransform;
            [Range(0, 360)] public float moveAngle = 0f; //0度为向右，90度为向上
        }

        [SerializeField] private SectionAnimationConfig[] sectionConfigs;
        [SerializeField] private float moveDistance = 1000f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private AnimationCurve enterCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve exitCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector2[] originalPositions;
        private Coroutine[] currentAnimations;

        protected override void Awake()
        {
            base.Awake();
            originalPositions = new Vector2[sectionConfigs.Length];
            currentAnimations = new Coroutine[sectionConfigs.Length];

            for (int i = 0; i < sectionConfigs.Length; i++)
            {
                originalPositions[i] = sectionConfigs[i].rectTransform.anchoredPosition;
            }
        }

        public override void PlayEnterAnimation()
        {
            gameObject.SetActive(true);
            //设置初始位置
            SetSectionsPosition(true);
            StartAnimations(true);
        }


        public override void PlayExitAnimation()
        {
            StartAnimations(false);
        }

        private void StartAnimations(bool isEnter)
        {
            for (int i = 0; i < sectionConfigs.Length; i++)
            {
                if (currentAnimations[i] != null)
                    StopCoroutine(currentAnimations[i]);
                currentAnimations[i] = StartCoroutine(AnimatePosition(i, isEnter, isEnter ? enterDelay : exitDelay));
            }
        }

        private void SetSectionsPosition(bool isEnter)
        {
            for (int i = 0; i < sectionConfigs.Length; i++)
            {
                Vector2 moveDirection = GetDirectionFromAngle(sectionConfigs[i].moveAngle);
                Vector2 targetPos = isEnter ?
                    originalPositions[i] + moveDirection * moveDistance :
                    originalPositions[i];
                sectionConfigs[i].rectTransform.anchoredPosition = targetPos;
            }
        }

        private IEnumerator AnimatePosition(int index, bool isEnter, float delay)
        {
            isAnimating = true;
            yield return new WaitForSeconds(delay);
            var config = sectionConfigs[index];
            Vector2 moveDirection = GetDirectionFromAngle(config.moveAngle);
            Vector2 startPos = isEnter ?
                originalPositions[index] + moveDirection * moveDistance :
                originalPositions[index];
            Vector2 endPos = isEnter ?
                originalPositions[index] :
                originalPositions[index] + moveDirection * moveDistance;

            float elapsed = 0;
            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                config.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos,
                    isEnter ? enterCurve.Evaluate(t) : exitCurve.Evaluate(t));
                yield return null;
            }

            config.rectTransform.anchoredPosition = endPos;
            currentAnimations[index] = null;

            if (IsAllAnimationsComplete())
            {
                OnAnimationComplete(isEnter);
                if (!isEnter)
                    gameObject.SetActive(false);
            }
        }

        private bool IsAllAnimationsComplete()
        {
            for (int i = 0; i < currentAnimations.Length; i++)
            {
                if (currentAnimations[i] != null)
                    return false;
            }
            return true;
        }
        private Vector2 GetDirectionFromAngle(float angle)
        {
            float radian = angle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

    }

}