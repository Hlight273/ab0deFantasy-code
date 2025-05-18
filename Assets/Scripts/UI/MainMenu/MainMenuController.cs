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
        [SerializeField] private GameObject mainMenuUI;  // �������Ұ�ť������˵�
        [SerializeField] private GameObject saveUI;  // �浵����
        [SerializeField] private GameObject multiplayUI;   // ������Ϸ�����б����

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
            { MainMenuState.MultiPlayerSaveSelect, saveUI }
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
            // ���ص�ǰUI
            if (currentUI != null)
            {
                var exitAnim = currentUI.GetComponent<UIAnimationBase>();
                if (exitAnim != null)
                    exitAnim.PlayExitAnimation();
            }

            // ��ʾ��UI
            if (stateUIMap.TryGetValue(newState, out GameObject newUI))
            {
                newUI.SetActive(true);
                var enterAnim = newUI.GetComponent<UIAnimationBase>();
                if (enterAnim != null)
                    enterAnim.PlayEnterAnimation(0.1f);

                currentUI = newUI;
            }

            // ����״̬����
            if (newState == MainMenuState.Main && NetworkManager.Instance != null)
            {
                NetworkManager.Instance.Disconnect();
            }

        }

        // private void HandleStateChanged(MainMenuState newState)
        // {

        //switch (newState)
        //{
        //    case MainMenuState.SinglePlayer:
        //        ShowSingleplaySelection();
        //        break;
        //    case MainMenuState.MultiPlayer:
        //        ShowMutiplaySelection();
        //        break;
        //    case MainMenuState.MultiPlayerSaveSelect:
        //        ShowMultiPlayerSaveSelection();
        //        break;
        //    case MainMenuState.Main:
        //        ShowMainMenu();
        //        break;
        //}
        // }
        //private void ShowSingleplaySelection()
        //{
        //    saveGroupAnimation.gameObject.SetActive(true);
        //    leftGroupAnimation.PlayExitAnimation();
        //    rightGroupAnimation.PlayExitAnimation();
        //    saveGroupAnimation.PlayEnterAnimation(0.1f);

        //}

        //private void ShowMutiplaySelection()
        //{
        //    if (model.CurrentState == MainMenuState.MultiPlayerSaveSelect)//�Ӷ��˴浵����
        //    {
        //        mutiplayplayGroupAnimation.gameObject.SetActive(true);
        //        mutiplayplayGroupAnimation.PlayEnterAnimation(0.1f);
        //    }
        //    else
        //    {
        //        mutiplayplayGroupAnimation.gameObject.SetActive(true);//���������ȥ
        //        leftGroupAnimation.PlayExitAnimation();
        //        rightGroupAnimation.PlayExitAnimation();
        //        mutiplayplayGroupAnimation.PlayEnterAnimation(0.1f);
        //    }

        //}



        //public void ShowMultiPlayerSaveSelection()
        //{
        //    saveGroupAnimation.gameObject.SetActive(true);
        //    mutiplayplayGroupAnimation.gameObject.SetActive(false);
        //    multiPlayerControllerObject.SetActive(true);
        //    //leftGroupAnimation.PlayExitAnimation();
        //    //rightGroupAnimation.PlayExitAnimation();
        //    mutiplayplayGroupAnimation.PlayExitAnimation();
        //    saveGroupAnimation.PlayEnterAnimation(0.1f);

        //}

        //private void ShowMainMenu()
        //{
        //    leftGroupAnimation.PlayEnterAnimation();
        //    rightGroupAnimation.PlayEnterAnimation();
        //    if (saveGroupAnimation.gameObject.activeSelf)
        //        saveGroupAnimation.PlayExitAnimation();
        //    if (mutiplayplayGroupAnimation.gameObject.activeSelf)
        //    {
        //        mutiplayplayGroupAnimation.PlayExitAnimation();
        //        //�������˵�ʱ�Ͽ���������
        //        if (NetworkManager.Instance != null)
        //        {
        //            NetworkManager.Instance.Disconnect();
        //        }
        //        ////���ض�����Ϸ������
        //        //if (multiPlayerControllerObject != null)
        //        //{
        //        //    multiPlayerControllerObject.SetActive(false);
        //        //}
        //    }
        //}



        private void ValidateSaveGroup()
        {
            for (int i = 0; i < singleplayGroup.Length; i++)
            {
                if (singleplayGroup[i] == null)
                {
                    Debug.LogError($"[MainMenuController] SaveGroup ��λ {i} δ���� SaveSlotUI ʵ�������� Inspector ���������д浵��λ��");
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.DisplayDialog(
                        "SaveSlot ���ô���",
                        $"SaveGroup ��λ {i} δ���� SaveSlotUI ʵ����\n���� Inspector ���������д浵��λ��",
                        "ȷ��"
                    );
                    //UnityEditor.EditorApplication.isPlaying = false;
#endif
                    return;
                }
            }
        }
    }
}