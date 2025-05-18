using HFantasy.Script.Network;
using HFantasy.Script.Network.Protocol;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI.MultiPlayer
{
    public class RoomItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.cyan;

        private RoomInfo roomInfo;
        public RoomInfo RoomInfo => roomInfo;

        public event Action<RoomInfo> OnRoomSelected;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => OnRoomSelected?.Invoke(roomInfo));
            backgroundImage.color = normalColor;
        }

        public void SetRoomInfo(RoomInfo info)
        {
            roomInfo = info;
            roomNameText.text = info.Name;
            playerCountText.text = $"{info.CurrentPlayers}/{info.MaxPlayers}";
        }

        public void SetSelected(bool selected)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
    }
}