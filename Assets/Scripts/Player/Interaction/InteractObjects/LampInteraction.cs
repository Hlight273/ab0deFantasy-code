using HFantasy.Script.Entity.Player;
using UnityEngine;

namespace HFantasy.Script.Player.Interaction
{
    public class LampInteraction : MonoBehaviour, IInteractable
    {
        public Light lampLight;  //�ƹ����
        private bool isOn = false;    //�Ƶ�״̬

        public string Description => "���صƹ�";

        private void Awake()
        {
            isOn = lampLight.enabled; //��ȡ�Ƶĳ�ʼ״̬
        }
        public void Interact(PlayerEntity playerEntity)
        {
            isOn = !isOn; 
            lampLight.enabled = isOn; //�л��Ƶ�״̬
        }
    }
}