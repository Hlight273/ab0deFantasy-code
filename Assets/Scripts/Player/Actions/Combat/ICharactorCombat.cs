namespace HFantasy.Script.Player.Actions.Combat
{
    public interface ICharactorCombat
    {
        bool IsAttacking { get; }
        bool IsCombatState { get; }
        bool AttackPressed { get; }
        void PerformAttack();
    }
}