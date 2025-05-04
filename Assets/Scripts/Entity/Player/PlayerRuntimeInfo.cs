using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerRuntimeInfo
    {
        public PlayerBattleInfo BattleInfo { get; }
        public bool IsHitstun => hitStunDuration > 0;
        public float HitstunTimer { get; set; } = 0f;//僵直计时器



        public float AttackTimer { get ; set; }//可攻击冷却计时器

        public float AttackRecoveryTimer { get; set; }//后摇计时器

        public float CombatStateTimer { get ; set ; }//战斗状态计时器

        public bool CanAttack => AttackTimer <= 0;


        public readonly float combatStateDuration = 4.5f;//战斗时间 超过之后停止战斗动作
        public readonly float attackRecoveryDuration = 0.5f;//平a后摇
        public float attackDuration => BattleInfo?.AttackInterval ?? float.PositiveInfinity;
        public float hitStunDuration = 0f;

        public PlayerRuntimeInfo(int lv)
        {
            BattleInfo = new PlayerBattleInfo(lv);
        }

        public float GetAttackCooldownValue()
        {
            return Mathf.Max(0, attackDuration - AttackTimer);
        }
    }
}
