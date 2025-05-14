using DG.Tweening;
using HFantasy.Script.Entity.Player;
using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class DoorInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField]private float openRotationAngle = -90f; //�Ŵ�ʱ����ת�Ƕ�
        [SerializeField]private float duration = 1f; //��������ʱ��
        [SerializeField]private Ease easeType = Ease.OutBounce; //��������

        private bool isOpen = false; //�ŵ�״̬
        private Vector3 initAngle = Vector3.zero;

        public string Description => "������";

        private void Awake()
        {
            initAngle = transform.rotation.eulerAngles;
        }

        public void Interact(PlayerEntity playerEntity)
        {
            float targetAngle = isOpen ? 0f : openRotationAngle;
            isOpen = !isOpen;

            //��ת������
            transform.DORotate(initAngle + new Vector3(0, targetAngle, 0), duration)
                     .SetEase(easeType);
        }
    }
}