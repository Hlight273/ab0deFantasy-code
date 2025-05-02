using HFantasy.Script.Player.StateMachine;
using UnityEngine;

namespace HFantasy.Script.Player.Movement
{
    public interface ICharactorMovement
    {
        // 基础状态
        bool IsGrounded { get; }
        bool IsJumpping { get; }


        Vector2 MoveInput { get; }
        bool JumpPressed { get; }

        // 核心行为
        void Move(Vector2 input);
        void Jump();
        void SetAnimation(PlayerStateType state);
        void ResetJump();

        void HandlePhysics();
    }
}