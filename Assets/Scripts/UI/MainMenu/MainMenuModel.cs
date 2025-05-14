using System;
using HFantasy.Script.Core.Save;
using HFantasy.Script.Entity;
using UnityEngine;

namespace HFantasy.Script.UI.MainMenu
{
    public class MainMenuModel
    {
        public MainMenuState CurrentState { get; private set; } = MainMenuState.Main;
        public event Action<MainMenuState> OnStateChanged;


        private SaveData[] saveSlots;
        public SaveData[] SaveSlots => saveSlots;


        public void LoadSaveSlots(SaveSlotUI[] saveGroup)
        {
            saveSlots = new SaveData[SaveSystem.MaxSaveSlots];
            for (int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i] = SaveSystem.Load(i);
                saveGroup[i].SetData(i, saveSlots[i]);
            }
        }

        public void ChangeState(MainMenuState newState)
        {
            if (CurrentState != newState)
            {
                CurrentState = newState;
                OnStateChanged?.Invoke(newState);
            }
        }
    }
}