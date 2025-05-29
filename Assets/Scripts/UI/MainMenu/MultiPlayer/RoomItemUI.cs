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
        [SerializeField] private Button selectButton;

        private RoomInfo roomInfo;

        public RoomInfo RoomInfo { get => roomInfo; set => roomInfo = value; }

        public event Action<RoomInfo> OnRoomSelected;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClicked);
        }
        private void OnDestroy()
        {
            selectButton.onClick.RemoveListener(OnSelectButtonClicked);
        }
        private void OnSelectButtonClicked()
        {
            if (RoomInfo == null) return;
            OnRoomSelected?.Invoke(RoomInfo);
        }


        public void SetRoomInfo(RoomInfo info)
        {
            RoomInfo = info;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (RoomInfo == null) return;

            roomNameText.text = RoomInfo.name;
            playerCountText.text = $"{RoomInfo.curPlayerCount}/{RoomInfo.maxPlayerCount}";
            selectButton.interactable = RoomInfo.curPlayerCount < RoomInfo.maxPlayerCount;
        }

    }
}