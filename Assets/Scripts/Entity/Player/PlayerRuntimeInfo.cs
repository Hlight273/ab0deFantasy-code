namespace HFantasy.Script.Entity.Player
{
    public class PlayerRuntimeInfo
    {
        public PlayerBattleInfo BattleInfo { get; }
        public bool IsHitstun => hitStunDuration > 0;
        public float HitstunTimer { get; set; } = 0f;



        public float AttackTimer { get ; set; }
        public float CombatStateTimer { get ; set ; }


        public readonly float combatStateDuration = 4.5f;//ս��ʱ�� ����֮��ֹͣս������
        public float attackDuration => BattleInfo?.AttackSpeed / 10f ?? float.PositiveInfinity;
        public float hitStunDuration = 0f;

        public PlayerRuntimeInfo(int lv)
        {
            BattleInfo = new PlayerBattleInfo(lv);
        }
       /* public PlayerRuntimeInfo()
        {
            
        }*/
    }
}