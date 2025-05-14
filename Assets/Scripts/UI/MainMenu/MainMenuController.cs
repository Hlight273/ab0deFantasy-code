using HFantasy.Script.UI.MainMenu.Animations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Main Menu Buttons")]
        [SerializeField] private Button singlePlayerButton;
        [SerializeField] private Button multiPlayerButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button backMainButton;

        [SerializeField] private SaveSlotUI[] saveGroup = new SaveSlotUI[3];

        [Header("UI Animations")]
        [SerializeField] private MainMenuButtonGroupAnimation leftGroupAnimation;
        [SerializeField] private MainMenuButtonGroupAnimation rightGroupAnimation;
        [SerializeField] private SaveSlotGroupAnimation saveSlotGroupAnimation;
        


        private MainMenuModel model;

        private void Awake()
        {
            ValidateSaveGroup();
            model = new MainMenuModel();
            model.OnStateChanged += HandleStateChanged;

            //按钮事件
            BindButtonEvents();
        }

        private void BindButtonEvents()
        {
            singlePlayerButton.onClick.AddListener(OnSinglePlayerClicked);
            multiPlayerButton.onClick.AddListener(OnMultiPlayerClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            backMainButton.onClick.AddListener(OnBackMainClicked);
        }

        private void OnDestroy()
        {
            if (model != null)
            {
                model.OnStateChanged -= HandleStateChanged;
            }
        }

        private void OnSinglePlayerClicked()
        {
            model.LoadSaveSlots(saveGroup);
            model.ChangeState(MainMenuState.SinglePlayer);
        }

        private void OnMultiPlayerClicked()
        {
            model.ChangeState(MainMenuState.MultiPlayer);
        }

        private void OnSettingsClicked()
        {
            model.ChangeState(MainMenuState.Settings);
        }

        private void OnBackMainClicked()
        {
            model.ChangeState(MainMenuState.Main);
        }

        private void HandleStateChanged(MainMenuState newState)
        {
            switch (newState)
            {
                case MainMenuState.SinglePlayer:
                    ShowSaveSlotSelection();
                    break;
                case MainMenuState.Main:
                    ShowMainMenu();
                    break;
            }
        }

        private void ShowSaveSlotSelection()
        {
            leftGroupAnimation.PlayExitAnimation();
            rightGroupAnimation.PlayExitAnimation();
            saveSlotGroupAnimation.PlayEnterAnimation(0.1f);
        }

        private void ShowMainMenu()
        {
            leftGroupAnimation.PlayEnterAnimation();
            rightGroupAnimation.PlayEnterAnimation();
            saveSlotGroupAnimation.PlayExitAnimation();
        }



        private void ValidateSaveGroup()
        {
            for (int i = 0; i < saveGroup.Length; i++)
            {
                if (saveGroup[i] == null)
                {
                    Debug.LogError($"[MainMenuController] SaveGroup 槽位 {i} 未设置 SaveSlotUI 实例！请在 Inspector 中设置所有存档槽位。");
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.DisplayDialog(
                        "SaveSlot 设置错误",
                        $"SaveGroup 槽位 {i} 未设置 SaveSlotUI 实例！\n请在 Inspector 中设置所有存档槽位。",
                        "确定"
                    );
                    //UnityEditor.EditorApplication.isPlaying = false;
#endif
                    return;
                }
            }
        }
    }
}