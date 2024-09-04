using UniRx;
using UnityEngine;

public abstract class BattleParticipantBaseController : CachedMonoBehaviour,
    IBattleParticipantParameters, IUpdateable<UpdateElementModel>
{
    [SerializeField] protected int clanId;
    
    public IReactiveProperty<int> Shield { get; }
    public IReactiveProperty<int> Health { get; }
    public ITransform BotTransform => this;
    public IReactiveProperty<bool> IsDeath { get; }
    public int ClanId => clanId;
}