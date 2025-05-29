using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HFantasy.Script.Core.Save;
using HFantasy.Script.Entity;
using HFantasy.Script.Core;
using HFantasy.Script.Common.Constant;
using HFantasy.Script.UI.MultiPlayer;

namespace HFantasy.Script.UI.MainMenu
{
    public class SaveSlotUI : MonoBehaviour
    {
        [SerializeField] private GameObject NoSave;
        [SerializeField] private Button createNewSaveButton;

        [SerializeField] private GameObject HasSave;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI lastSaveTimeText;
        [SerializeField] private Button playButton;
        [SerializeField] private Button deleteButton;

        private bool isMultiPlayerMode => MainMenuUIManager.Instance.MainMenuModel.CurrentState== MainMenuState.MultiPlayerSaveSelect;

        public void SetData(int saveIndex, SaveData saveData)
        {
            if (saveData != null)
            {
                NoSave.SetActive(false);
                HasSave.SetActive(true);
                playerNameText.text = saveData.myPlayerInfo.Name;
                levelText.text = $"Lv {saveData.myPlayerInfo.LV}";
                lastSaveTimeText.text = saveData.lastSaveTime;
                playButton.onClick.RemoveAllListeners();
                createNewSaveButton.onClick.RemoveAllListeners();
                deleteButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => LoadGame(saveIndex, saveData));
                deleteButton.onClick.AddListener(() => DeleteSave(saveIndex));
            }
            else
            {
                NoSave.SetActive(true);
                HasSave.SetActive(false);
                playerNameText.text = "Empty Slot";
                levelText.text = "";
                lastSaveTimeText.text = "";
                playButton.onClick.RemoveAllListeners();
                createNewSaveButton.onClick.RemoveAllListeners();
                deleteButton.onClick.RemoveAllListeners();
                createNewSaveButton.onClick.AddListener(() => CreateSave(saveIndex));
            }
        }

        private void LoadGame(int saveIndex, SaveData saveData)
        {
            SaveSystem.SelectSaveAndEnterGame(saveIndex, saveData);
            if (isMultiPlayerMode)
            {
                // 多人模式
                //SceneController.Instance.SwitchScene(SceneConstant.ThreeDLobby);
                MainMenuUIManager.Instance.MultiplayerUIController.OnSaveSelect?.Invoke(saveData);
            }
            else
            {
                // 单人模式下直接进入游戏
                SceneController.Instance.SwitchScene(SceneConstant.ThreeDLobby);
            }
        }

        private void CreateSave(int saveIndex)
        {
            SaveData newSaveData = SaveSystem.CreateNewSave(saveIndex);
            if (newSaveData != null)
            {
                SetData(saveIndex, newSaveData);
            }
        }

        private void DeleteSave(int saveIndex)
        {
            SaveSystem.Delete(saveIndex);
            SetData(saveIndex, null);
        }


    }
}