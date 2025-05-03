//namespace HFantasy.Script.Player.Combat
//{
//    using HFantasy.Script.Player.Combat;
//    using UnityEngine;
//    using UnityEngine.InputSystem;
//    using UnityEngine.EventSystems;
//    using HFantasy.Script.Player.Movement;
//    using HFantasy.Script.Core;

//    public class PlayerAttack : MonoBehaviour
//    {
//        private PlayerControls controls;
//        private ICharactorMovement movement;
//        private void Awake()
//        {
//            controls = new PlayerControls();
//            movement = GetComponent<ICharactorMovement>();
//        }
//        private void OnEnable()
//        {
//            InputManager.Instance.OnAttackPressed += HandleAttack;
//        }
//        private void OnDisable()
//        {
//            InputManager.Instance.OnAttackPressed -= HandleAttack;
//        }
//        private void HandleAttack()
//        {
//            // ²¥·Å¹¥»÷¶¯»­¡¢¼ì²âÃüÖÐµÈ
//        }
//    }
//}