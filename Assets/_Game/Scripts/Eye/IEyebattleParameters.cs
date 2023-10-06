using UniRx;

public interface IEyebattleParameters
{
    public IReactiveProperty<int> KillCount { get; }
    
}