using UnityEngine;
using HFantasy.Script.UI.MainMenu;
using HFantasy.Script.Common;

namespace HFantasy.Script.UI.MainMenu
{
    [NonPersistentSingletonAttribute]
    public class MainMenuUIManager : MonoSingleton<MainMenuUIManager>
{
       

        private MainMenuModel mainMenuModel;
        public MainMenuModel MainMenuModel => mainMenuModel ??= new MainMenuModel();

        

       
    }
}