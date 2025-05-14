using HFantasy.Script.Core;
using HFantasy.Script.Entity.Player;
using HFantasy.Script.Player.Actions;
using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactionDistance = 2f;  //交互距离
        public LayerMask interactableLayer;     //可交互物体的物理层

        private void OnEnable()
        {
            InputManager.Instance.OnInteractPressed += TryInteract;
        }
        private void OnDisable()
        {
            InputManager.Instance.OnInteractPressed -= TryInteract;
        }

        private void Update()
        {
            //获取鼠标位置并转换为世界坐标
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
            {
                //Debug.LogWarning(hit.transform.gameObject.name);
                //更新状态
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    InputManager.Instance.SetInteractableTarget(true, interactable.Description);
                }
                else
                {
                    InputManager.Instance.SetInteractableTarget(false);
                }
            }
            else
            {
                InputManager.Instance.SetInteractableTarget(false);
            }
        }

        private void TryInteract()
        {
            if (!InputManager.Instance.HasInteractableTarget) return;

            PlayerEntity playerEntity = gameObject.GetComponent<PlayerActions>().PlayerEntity;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(playerEntity);
                }
            }
        }
    }
}
