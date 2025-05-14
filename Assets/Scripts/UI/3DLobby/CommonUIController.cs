using HFantasy.Script.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy
{
    
    public class CommonUIController : MonoBehaviour
    {
        private const string MENU_NAME = "其他通用UI";
        [Header(MENU_NAME)]
        [Space(10)]

        [SerializeField] private Image InteractTips;
        [SerializeField] private TextMeshProUGUI DescriptionText;

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
                    DescriptionText.text = InputManager.Instance.InteractableTargetInfo;
                }
                else
                {
                    InteractTips.gameObject.SetActive(false);
                }
            }
        }
    }
}
