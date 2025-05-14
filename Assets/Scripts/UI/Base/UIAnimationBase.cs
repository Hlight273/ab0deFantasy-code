using UnityEngine;
using HFantasy.Script.UI.Interfaces;

namespace HFantasy.Script.UI.Base
{
    public abstract class UIAnimationBase : MonoBehaviour, IUIAnimatable
    {
        protected bool isAnimating;
        public bool IsAnimating => isAnimating;

        protected virtual void Awake()
        {
            //��ʼ��ʱ�������ó�ʼ״̬
        }

        public abstract void PlayEnterAnimation(float delay = 0f);
        public abstract void PlayExitAnimation(float delay = 0f);

        protected virtual void OnAnimationComplete(bool isEnter)
        {
            isAnimating = false;
        }
    }
}