using UnityEngine;
using HFantasy.Script.BattleSystem.Damage;

namespace HFantasy.Script.BattleSystem.Events
{
    public struct CombatEventData
    {
        public enum EventType
        {
            AttackStart,
            AttackHit,
            DamageTaken,
            Death
        }

        public EventType Type { get; private set; }
        public GameObject Source { get; private set; }
        public GameObject Target { get; private set; }
        public DamageInfo DamageInfo { get; private set; }

        public CombatEventData(EventType type, GameObject source, GameObject target = null, DamageInfo damageInfo = null)
        {
            Type = type;
            Source = source;
            Target = target;
            DamageInfo = damageInfo;
        }
    }
}