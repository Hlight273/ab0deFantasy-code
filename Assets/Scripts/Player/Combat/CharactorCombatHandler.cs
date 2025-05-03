namespace HFantasy.Script.Player.Combat
{
    using HFantasy.Script.Common.Constant;
    using HFantasy.Script.Core;
    using HFantasy.Script.Player.StateMachine;
    using UnityEngine;
    public class CharactorCombatHandler : ICharactorCombat
    {
        private readonly CharacterController controller;
        private readonly Animator animator;

        private float boxingTimer;
        public float boxingTimeMax = 200f;

        public CharactorCombatHandler(CharacterController controller, Animator animator) {
            this.controller = controller;
            this.animator = animator;
            boxingTimer = 0;
        }

     

        public bool IsAttacking
        {
           get
           {
                if (boxingTimer > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
           }
        }
        public void ResetBoxingTime()
        {
            boxingTimer = boxingTimeMax;
        }
        public void Attack()
        {
            Debug.LogWarning("attack");
        }

        public void SetAnimation(PlayerCombatState state)
        {
            if (animator == null) return;

            // 先重置所有动画状态
            animator.SetBool(AnimationConstant.BoxingHit, false);
            animator.SetBool(AnimationConstant.BoxingKick, false);
            animator.SetBool(AnimationConstant.Hit, false);
            animator.SetBool(AnimationConstant.Death, false);
            animator.SetBool(AnimationConstant.Boxing, false);

            // 设置当前状态
            switch (state)
            {
                case PlayerCombatState.BoxingHit:
                    animator.SetBool(AnimationConstant.BoxingHit, true);
                    animator.SetBool(AnimationConstant.Boxing, true);
                    break;
                case PlayerCombatState.BoxingKick:
                    animator.SetBool(AnimationConstant.BoxingKick, true);
                    animator.SetBool(AnimationConstant.Boxing, true);
                    break;
                case PlayerCombatState.Hit:
                    animator.SetBool(AnimationConstant.Hit, true);
                    break;
                case PlayerCombatState.Death:
                    animator.SetBool(AnimationConstant.Death, true);
                    break;
                case PlayerCombatState.Boxing:

                    break;
            }
        }

       
    }
}