namespace HFantasy.Script.Player.Combat
{
    using HFantasy.Script.Core;
    using HFantasy.Script.Player.StateMachine;
    using UnityEngine;
    public interface ICharactorCombat
    {
        // 基础状态
        bool IsAttacking { get; }
        //bool IsBlocking { get; }
        //bool IsDodging { get; }
        //bool IsParrying { get; }
        // 核心行为
        void Attack();

        void ResetBoxingTime();
        //void Block();
        //void Dodge();
        //void Parry();
        void SetAnimation(PlayerCombatState state);



    }
}