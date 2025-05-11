using HFantasy.Script.Core;

using UnityEngine;
using UnityEngine.UI;

namespace HFantasy
{
    public class CommonUIController : MonoBehaviour
    {
        [SerializeField] private Image InteractTips;

        private void Start()
        {
            InteractTips.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (InputManager.Instance != null)
            {
                if (InputManager.Instance.HasInteractableTarget)
                {
                    InteractTips.gameObject.SetActive(true);
                }
                else
                {
                    InteractTips.gameObject.SetActive(false);
                }
            }
        }
    }
}
