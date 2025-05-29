using UnityEngine;
using HFantasy.Script.UI.MainMenu;
using HFantasy.Script.Common;
using HFantasy.Script.UI.MultiPlayer;

namespace HFantasy.Script.UI.MainMenu
{
    [NonPersistentSingletonAttribute]
    public class MainMenuUIManager : MonoSingleton<MainMenuUIManager>
{
       

        private MainMenuModel mainMenuModel;
        public MainMenuModel MainMenuModel => mainMenuModel ??= new MainMenuModel();

        [SerializeField]
        private MultiplayerUIController multiplayerUIControllerGo;
        public MultiplayerUIController MultiplayerUIController => multiplayerUIControllerGo;




    }
}