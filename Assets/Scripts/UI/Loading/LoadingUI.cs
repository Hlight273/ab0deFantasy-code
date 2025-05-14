using UnityEngine;
using UnityEngine.UI;
using HFantasy.Script.Core;
using TMPro;

namespace HFantasy.Script.UI.Loading
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI textNum;

        private void OnEnable()
        {
            SceneController.Instance.OnLoadingProgressChanged += UpdateProgress;
        }

        private void OnDisable()
        {
            SceneController.Instance.OnLoadingProgressChanged -= UpdateProgress;
        }

        private void UpdateProgress(float resourceProgress, float sceneProgress)
        {
            if (resourceProgress < 1f)
            {
                progressText.text = ("Loading Resources...");
                progressImage.fillAmount = resourceProgress;
                textNum.text = ($"{resourceProgress * 100:0}%");
            }
            else if (sceneProgress < 1f)
            {
                progressText.text = ("Loading Scene...");
                progressImage.fillAmount = sceneProgress;
                textNum.text = ($"{sceneProgress * 100:0}%");

            }
        }
    }
}
