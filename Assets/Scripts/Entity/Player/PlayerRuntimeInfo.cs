using UnityEngine;

namespace HFantasy.Script.Entity.Player
{
    public class PlayerRuntimeInfo
    {
        public PlayerBattleInfo BattleInfo { get; }
        public bool IsHitstun => hitStunDuration > 0;
        public float HitstunTimer { get; set; } = 0f;//��ֱ��ʱ��



        public float AttackTimer { get ; set; }//�ɹ�����ȴ��ʱ��

        public float AttackRecoveryTimer { get; set; }//��ҡ��ʱ��

        public float CombatStateTimer { get ; set ; }//ս��״̬��ʱ��

        public bool CanAttack => AttackTimer <= 0;


        public readonly float combatStateDuration = 4.5f;//ս��ʱ�� ����֮��ֹͣս������
        public readonly float attackRecoveryDuration = 0.5f;//ƽa��ҡ
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
