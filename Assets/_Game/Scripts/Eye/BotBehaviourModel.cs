using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BotBehaviourModel", menuName = "Data/Bot/Model")]
public class BotBehaviourModel : ScriptableObject
{
    public float AttackRadius;
    public float Distance;
    public float Speed;
}