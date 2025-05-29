using System;
using System.Collections.Generic;
using HFantasy.Script.Core.Save;
using HFantasy.Script.Entity;
using UnityEngine;

namespace HFantasy.Script.UI.MainMenu
{
    public class MainMenuModel
    {
        private Stack<MainMenuState> stateStack = new Stack<MainMenuState>();
        public MainMenuState CurrentState => stateStack.Count > 0 ? stateStack.Peek() : MainMenuState.Main;
        public event Action<MainMenuState> OnStateChanged;

        private SaveData[] saveSlots;
        public SaveData[] SaveSlots => saveSlots;

        public void PushState(MainMenuState newState)
        {
            if (stateStack.Count == 0 || stateStack.Peek() != newState)
            {
                stateStack.Push(newState);
                OnStateChanged?.Invoke(newState);
            }
        }

        public void PopState()
        {
            if (stateStack.Count > 1)
            {
                stateStack.Pop();
                OnStateChanged?.Invoke(stateStack.Peek());
            }
            else if (stateStack.Count == 1)
            {
                // 如果是最后一个状态，回到主菜单
                stateStack.Clear();
                PushState(MainMenuState.Main);
            }
        }



        public void LoadSaveSlots(SaveSlotUI[] saveGroup)
        {
            saveSlots = new SaveData[SaveSystem.MaxSaveSlots];
            for (int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i] = SaveSystem.Load(i);
                saveGroup[i].SetData(i, saveSlots[i]);
            }
        }

        // public void ChangeState(MainMenuState newState)
        // {
        //     if (CurrentState != newState)
        //     {
        //         CurrentState = newState;
        //         OnStateChanged?.Invoke(newState);
        //     }
        // }
    }
}