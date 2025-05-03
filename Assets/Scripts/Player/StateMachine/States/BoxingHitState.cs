using HFantasy.Script.Player.Combat;
using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine {
    public class BoxingHitState : BasePlayerCombatState
    {
        public BoxingHitState(ICharactorCombat combat) : base(combat)
        {
        }

        public override void Enter()
        {
            combat.SetAnimation(PlayerCombatState.BoxingHit);
            combat.ResetBoxingTime();
        }

        public override void Exit()
        {
        }

        protected override void UpdateState()
        {
            combat.Attack();
        }
    }
}