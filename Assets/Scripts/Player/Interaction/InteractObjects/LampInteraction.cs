using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class LampInteraction : MonoBehaviour, IInteractable
    {
        public Light lampLight;  //�ƹ����
        private bool isOn = false;    //�Ƶ�״̬

        private void Awake()
        {
            isOn = lampLight.enabled; //��ȡ�Ƶĳ�ʼ״̬
        }
        public void Interact()
        {
            isOn = !isOn; 
            lampLight.enabled = isOn; //�л��Ƶ�״̬
        }
    }
}