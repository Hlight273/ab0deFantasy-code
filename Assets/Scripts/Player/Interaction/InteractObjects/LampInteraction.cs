using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class LampInteraction : MonoBehaviour, IInteractable
    {
        public Light lampLight;  //灯光对象
        private bool isOn = false;    //灯的状态

        private void Awake()
        {
            isOn = lampLight.enabled; //获取灯的初始状态
        }
        public void Interact()
        {
            isOn = !isOn; 
            lampLight.enabled = isOn; //切换灯的状态
        }
    }
}