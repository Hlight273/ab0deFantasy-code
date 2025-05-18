using HFantasy.Script.Network.Core;
using HFantasy.Script.UI.MainMenu.Animations;
using HFantasy.Script.UI.MultiPlayer;
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
        [SerializeField] private Button[] backMainButtons;

        [SerializeField] private SaveSlotUI[] singleplayGroup = new SaveSlotUI[3];

        [Header("UI Animations")]
        [SerializeField] private MainMenuButtonGroupAnimation leftGroupAnimation;
        [SerializeField] private MainMenuButtonGroupAnimation rightGroupAnimation;
        [SerializeField] private SaveSlotGroupAnimation singleplayGroupAnimation;
        [SerializeField] private SaveSlotGroupAnimation mutiplayplayGroupAnimation;

         [Header("多人游戏")]
        [SerializeField] private GameObject multiPlayerControllerObject;
        private MultiPlayerController multiPlayerController;


        private MainMenuModel model;

        private void Awake()
        {
            ValidateSaveGroup();
            model = new MainMenuModel();
            model.OnStateChanged += HandleStateChanged;
            
            if (multiPlayerControllerObject != null)
            {
                multiPlayerController = multiPlayerControllerObject.GetComponent<MultiPlayerController>();
                //multiPlayerControllerObject.SetActive(false);
            }

            BindButtonEvents();
        }

        private void BindButtonEvents()
        {
            singlePlayerButton.onClick.AddListener(OnSinglePlayerClicked);
            multiPlayerButton.onClick.AddListener(OnMultiPlayerClicked);
            settingsButton.onClick.AddListener(OnSettingsClicked);
            foreach (var backMainButton in backMainButtons)
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
            model.LoadSaveSlots(singleplayGroup);
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
                    ShowSingleplaySelection();
                    break;
                case MainMenuState.MultiPlayer:
                    ShowMutiplaySelection();
                    break;
                case MainMenuState.MultiPlayerSaveSelect:
                    ShowMultiPlayerSaveSelection();
                    break;
                case MainMenuState.Main:
                    ShowMainMenu();
                    break;
            }
        }

        private void ShowSingleplaySelection()
        {
            singleplayGroupAnimation.gameObject.SetActive(true);
            leftGroupAnimation.PlayExitAnimation();
            rightGroupAnimation.PlayExitAnimation();
            singleplayGroupAnimation.PlayEnterAnimation(0.1f);

        }

        private void ShowMutiplaySelection()
        {
            mutiplayplayGroupAnimation.gameObject.SetActive(true);
            leftGroupAnimation.PlayExitAnimation();
            rightGroupAnimation.PlayExitAnimation();
            mutiplayplayGroupAnimation.PlayEnterAnimation(0.1f);
        }



        public void ShowMultiPlayerSaveSelection()
        {
            singleplayGroupAnimation.gameObject.SetActive(true);
            mutiplayplayGroupAnimation.gameObject.SetActive(false);
            multiPlayerControllerObject.SetActive(true);
            leftGroupAnimation.PlayExitAnimation();
            rightGroupAnimation.PlayExitAnimation();
            singleplayGroupAnimation.PlayEnterAnimation(0.1f);
        }

        private void ShowMainMenu()
        {
             leftGroupAnimation.PlayEnterAnimation();
            rightGroupAnimation.PlayEnterAnimation();
            if (singleplayGroupAnimation.gameObject.activeSelf)
                singleplayGroupAnimation.PlayExitAnimation();
            if (mutiplayplayGroupAnimation.gameObject.activeSelf)
            {
                mutiplayplayGroupAnimation.PlayExitAnimation();
                //返回主菜单时断开网络连接
                if (NetworkManager.Instance != null)
                {
                    NetworkManager.Instance.Disconnect();
                }
                ////隐藏多人游戏控制器
                //if (multiPlayerControllerObject != null)
                //{
                //    multiPlayerControllerObject.SetActive(false);
                //}
            }
        }



        private void ValidateSaveGroup()
        {
            for (int i = 0; i < singleplayGroup.Length; i++)
            {
                if (singleplayGroup[i] == null)
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