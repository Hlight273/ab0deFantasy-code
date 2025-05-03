namespace HFantasy.Script.Player.StateMachine
{
    //public enum PlayerMovementState
    //{
    //    Idle,
    //    Walk,
    //    Jump,
    //    Run,

    //    Boxing,
    //    BoxingHit,
    //    BoxingKick,

    //    Hit,

    //    Death,
    //}

    public enum StateDomainType { Movement, Combat }

    public enum PlayerMovementState { Idle, Walk, Run, Jump }
    public enum PlayerCombatState { Boxing, BoxingHit, BoxingKick, Hit, Death }

}