using HFantasy.Script.Common.Constant;
using HFantasy.Script.Core;
using HFantasy.Script.Core.Save;
using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI
{
    public class OptionsMenuController: MonoBehaviour
    {
        private const string MENU_NAME = "左上角选项菜单UI";
        [Header(MENU_NAME)]
        [Space(10)]

        [SerializeField] private Button openButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private Button goMainMenuButton;

        [SerializeField] private GameObject optionsMenu;

        private void Start()
        {
            InitializeButtons();
        }
        private void InitializeButtons()
        {
            openButton.onClick.AddListener(() => OnOptionsMenuButtonClick(true));
            closeButton.onClick.AddListener(() => OnOptionsMenuButtonClick(false));

            goMainMenuButton.onClick.AddListener(() => OnGoMainMenuButtonClick());

        }

        private void OnOptionsMenuButtonClick(bool isOpen)
        {
            optionsMenu.SetActive(isOpen);
        }

        private void OnGoMainMenuButtonClick()
        {
            SceneController.Instance.SwitchScene(SceneConstant.MainMenu);
            OnOptionsMenuButtonClick(false);
            SaveSystem.ExitAndSaveGame();
        }


    }
}