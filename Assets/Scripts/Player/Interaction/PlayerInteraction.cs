using HFantasy.Script.Core;
using HFantasy.Script.Player.Interaction;
using UnityEngine;

namespace HFantasy
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactionDistance = 2f;  //��������
        public LayerMask interactableLayer;     //�ɽ�������������

        private void OnEnable()
        {
            InputManager.Instance.OnInteractPressed += TryInteract;
        }
        private void OnDisable()
        {
            InputManager.Instance.OnInteractPressed -= TryInteract;
        }


        private void TryInteract()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, interactionDistance, interactableLayer);

            if (hit.collider != null)
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();  // ���ý�������
                }
            }
        }
    }
}
