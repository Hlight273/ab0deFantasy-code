using DG.Tweening;
using HFantasy.Script.Entity.Player;
using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class DoorInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField]private float openRotationAngle = -90f; //门打开时的旋转角度
        [SerializeField]private float duration = 1f; //动画持续时间
        [SerializeField]private Ease easeType = Ease.OutBounce; //缓动类型

        private bool isOpen = false; //门的状态
        private Vector3 initAngle = Vector3.zero;

        public string Description => "开关门";

        private void Awake()
        {
            initAngle = transform.rotation.eulerAngles;
        }

        public void Interact(PlayerEntity playerEntity)
        {
            float targetAngle = isOpen ? 0f : openRotationAngle;
            isOpen = !isOpen;

            //旋转开关门
            transform.DORotate(initAngle + new Vector3(0, targetAngle, 0), duration)
                     .SetEase(easeType);
        }
    }
}