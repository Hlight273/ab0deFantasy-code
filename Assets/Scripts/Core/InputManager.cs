using HFantasy.Script.Common;
using System;
using UnityEngine;

namespace HFantasy.Script.Core
{
    public class InputManager : MonoSingleton<InputManager>
    {
        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }

        public event Action OnInteractPressed;

        private PlayerControls controls;

        protected override void Awake()
        {
            base.Awake();
            controls = new PlayerControls();

            controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;

            controls.Player.Jump.performed += _ => JumpPressed = true;

            controls.Player.Interact.performed += _ => OnInteractPressed?.Invoke();
        }

        private void LateUpdate()
        {
            JumpPressed = false; //Ã¿Ö¡ÖØÖÃ
        }

        private void OnEnable() => controls.Enable();
        private void OnDisable() => controls.Disable();
    }
}