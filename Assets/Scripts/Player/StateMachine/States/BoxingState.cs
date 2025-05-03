using HFantasy.Script.Player.Combat;
using HFantasy.Script.Player.Movement;
using HFantasy.Script.Player.StateMachine.States;

namespace HFantasy.Script.Player.StateMachine
{
    public class BoxingState : BasePlayerCombatState
    {
        public BoxingState(ICharactorCombat combat) : base(combat)
        {
        }

        public override void Enter()
        {
            combat.SetAnimation(PlayerCombatState.Boxing);
        }

        public override void Exit()
        {
        }

        protected override void UpdateState()
        {
        }
    }
}