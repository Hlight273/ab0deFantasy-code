using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI.MultiPlayer
{
    public class PlayerItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private GameObject hostIcon;
        
        public void SetPlayerInfo(string playerName, bool isHost)
        {
            playerNameText.text = playerName;
            hostIcon.SetActive(isHost);
        }
    }
}