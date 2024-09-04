using UniRx;

public interface IBotBattleParameters
{
    public IReactiveProperty<int> KillCount { get; }
    
}