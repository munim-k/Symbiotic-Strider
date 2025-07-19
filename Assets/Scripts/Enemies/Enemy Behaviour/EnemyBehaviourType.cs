using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourType
{
    public static Dictionary<BehaviourType, Type> behaviourClasses = new Dictionary<BehaviourType, Type>
    {
        { BehaviourType.Aggressive, typeof(AggressiveEnemyBehaviour) },
        { BehaviourType.Defensive, typeof(DefensiveEnemyBehaviour) },
        { BehaviourType.Hybrid, typeof(HybridEnemyBehaviour) }
    };
    public enum BehaviourType
    {
        Aggressive,
        Defensive,
        Hybrid
    }
}
