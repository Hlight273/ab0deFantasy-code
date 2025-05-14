using UnityEngine;
using UnityEngine.UI;

namespace HFantasy.Script.UI
{
    
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class ContextMenuController : MonoBehaviour
    {
        private const string MENU_NAME = "ÓÒÉÏ½Ç²Ëµ¥UI";
        [Header(MENU_NAME)]
        [Space(10)]

        [SerializeField] private Button[] menuButtons;
        [SerializeField] private Button contextDrawButton;
        [SerializeField] private DrawerMenu drawerMenu;

        private void Start()
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            foreach (var button in menuButtons)
            {
                button.onClick.AddListener(() => {
                    OnMenuButtonClick(button);
                });
            }
            contextDrawButton.onClick.AddListener(() => OnContextDrawButtonClick());
        }

        private void OnMenuButtonClick(Button button)
        {
            switch (button.name)
            {
                case "OptionsMenu":
                    
                    break;
                case "ProfileButton":
                    
                    break;
                    
            }
        }
        private void OnContextDrawButtonClick()
        {
            if (drawerMenu != null)
            {
                drawerMenu.ToggleDrawer();

            }
        }

        private void OnDestroy()
        {
            foreach (var button in menuButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}