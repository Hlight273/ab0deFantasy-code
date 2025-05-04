using UnityEngine;

namespace HFantasy.Script.Player.Actions.Movement
{
    public interface ICharactorMovement
    {
        Vector2 MoveInput { get; }
        bool JumpPressed { get; }
        bool IsGrounded { get; }
        bool IsJumping { get; }
        bool IsRunning { get; }
        float RunSpeedMultiplier { get; }

         void Move(Vector2 input);
        void Jump();
        void HandlePhysics();
        void ResetJump();
    }
}