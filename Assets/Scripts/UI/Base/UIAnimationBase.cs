using UnityEngine;
using HFantasy.Script.UI.Interfaces;

namespace HFantasy.Script.UI.Base
{
    public abstract class UIAnimationBase : MonoBehaviour, IUIAnimatable
    {
        [SerializeField] protected float enterDelay = 0f;
    [SerializeField] protected float exitDelay = 0f;
    public float EnterDelay => enterDelay;
    public float ExitDelay => exitDelay;

        protected bool isAnimating;
        public bool IsAnimating => isAnimating;

        protected virtual void Awake()
        {
            //��ʼ��ʱ�������ó�ʼ״̬
        }

        public abstract void PlayEnterAnimation();
        public abstract void PlayExitAnimation();

        protected virtual void OnAnimationComplete(bool isEnter)
        {
            isAnimating = false;
        }
    }
}