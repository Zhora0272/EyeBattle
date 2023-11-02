using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BotBehaviourModel", menuName = "Data/Bot/Model")]
public class BotBehaviourModel : ScriptableObject, ICloneable
{
    public float AttackRadius;
    public object Clone()
    {
        return new BotBehaviourModel
        {
            AttackRadius = this.AttackRadius,
        };
    }
}