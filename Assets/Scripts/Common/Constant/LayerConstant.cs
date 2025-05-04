namespace HFantasy.Script.Common.Constant
{
    public static class LayerConstant
    {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        public const int Ground = 3;
        public const int Water = 4;
        public const int UI = 5;
        public const int Player = 6;
        public const int Enemy = 7;
        public const int NPC = 8;

        public const int Projectile = 10;
        public const int Trigger = 11;

        public static class Masks
        {
            public static readonly int PlayerMask = 1 << Player;
            public static readonly int EnemyMask = 1 << Enemy;
            public static readonly int NPCMask = 1 << NPC;
            public static readonly int CanPlayerAttackMask = PlayerMask | EnemyMask | NPCMask;
            public static readonly int ExcludeIgnoreRaycastAndPlayerMask = ~(1 << IgnoreRaycast | 1 << Player);

        }

        
    }
}