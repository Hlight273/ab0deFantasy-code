using HFantasy.Script.Core;
using HFantasy.Script.Entity.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace HFantasy
{
    public class SimpleProgressUI : MonoBehaviour
    {
        private UnityEngine.UI.Slider slider;
        public void Start()
        {
            slider = GetComponent<UnityEngine.UI.Slider>();
        }
        private void Update()
        {
            PlayerEntity playerEntity = EntityManager.Instance?.MyPlayerEntity;
            if (playerEntity?.RuntimeInfo == null) return;

            var maxValue = playerEntity.RuntimeInfo.attackDuration;
            var value = Mathf.Max(0, maxValue - playerEntity.RuntimeInfo.AttackTimer);
            slider.maxValue = maxValue;
            slider.value = value;
        }
    }
}
