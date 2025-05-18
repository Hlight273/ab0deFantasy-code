using HFantasy.Script.Network.Core;
using HFantasy.Script.UI.Base;
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

        [Header("UI States")]
        [SerializeField] private GameObject mainMenuUI;    //包含左右按钮组的主菜单
        [SerializeField] private GameObject saveUI;        //存档界面
        [SerializeField] private GameObject multiplayUI;   //多人游戏房间列表界面
        [SerializeField] private GameObject settingsUI;    //设置界面

        private Dictionary<MainMenuState, GameObject> stateUIMap;
        private GameObject currentUI;

        private MainMenuModel model;

        private void Awake()
        {
            InitializeStateUIMap();
            ValidateSaveGroup();
            model = MainMenuUIManager.Instance.MainMenuModel;
            model.OnStateChanged += HandleStateChanged;
            currentUI = mainMenuUI;


            BindButtonEvents();
        }

        private void InitializeStateUIMap()
        {
            stateUIMap = new Dictionary<MainMenuState, GameObject>
            {
                { MainMenuState.Main, mainMenuUI },
                { MainMenuState.SinglePlayer, saveUI },
                { MainMenuState.MultiPlayer, multiplayUI },
                { MainMenuState.MultiPlayerSaveSelect, saveUI },
                { MainMenuState.Settings, settingsUI }
            };
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
            model.PushState(MainMenuState.SinglePlayer);
        }

        private void OnMultiPlayerClicked()
        {
            model.LoadSaveSlots(singleplayGroup, true);
            model.PushState(MainMenuState.MultiPlayer);
        }

        private void OnSettingsClicked()
        {
            model.PushState(MainMenuState.Settings);
        }

        private void OnBackMainClicked()
        {
            model.PopState();
        }

        private void HandleStateChanged(MainMenuState newState)
        {
            //隐藏当前UI
            if (currentUI != null)
            {
                var exitAnim = currentUI.GetComponent<UIAnimationBase>();
                if (exitAnim != null)
                    exitAnim.PlayExitAnimation();
            }

            //显示新UI
            if (stateUIMap.TryGetValue(newState, out GameObject newUI))
            {
                var enterAnim = newUI.GetComponent<UIAnimationBase>();
                if (enterAnim != null)
                    enterAnim.PlayEnterAnimation();

                currentUI = newUI;
            }

            //特殊状态处理
            if (newState == MainMenuState.Main && NetworkManager.Instance != null)
            {
                NetworkManager.Instance.Disconnect();
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