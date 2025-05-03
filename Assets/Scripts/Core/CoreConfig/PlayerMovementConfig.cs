using UnityEngine;

namespace HFantasy.Script.Core.CoreConfig
{
    [CreateAssetMenu(fileName = "MovementConfig", menuName = "创建配置/Movement")]
    public class PlayerMovementConfig : ScriptableObject
    {
        [Header("移动参数")]
        public float MoveSpeed = 1.74f;
        public float RunSpeed = 3.34f;

        [Header("跳跃参数")]
        public float JumpHeight = 0.2f;
        public float JumpRange = 2.48f;
        public float Gravity = -9.81f;

        [Header("起跳和到地面偏移时间")]
        public float JumpStartOffset = 0.75f;
        public float JumpEndOffset = 1.64f;
        public float JumpEndTime = 1.85f;

        [Header("地面检测")]
        public LayerMask GroundLayer;
        public float GroundDistance = 0.4f;
    }
}