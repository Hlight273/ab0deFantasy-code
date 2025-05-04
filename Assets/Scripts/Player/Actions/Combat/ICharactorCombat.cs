namespace HFantasy.Script.Player.Actions.Combat
{
    public interface ICharactorCombat
    {
        bool IsAttacking { get; }

        bool IsAttackRecovering { get; }

        bool IsCombatState { get; }

        bool CanAttack { get; }

        void PerformAttack();
    }
}