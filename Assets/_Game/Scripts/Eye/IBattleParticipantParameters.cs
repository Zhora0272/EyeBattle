using UniRx;
using UnityEngine;

public interface IBattleParticipantParameters
{
    public IReactiveProperty<int> Shield { get; }
    public IReactiveProperty<int> Health { get; }
    public Vector3 Position { get; }
    public ITransform BotTransform { get; }
    public IReactiveProperty<bool> IsDeath { get; }
    public int ClanId { get; }
}