using UniRx;
using UnityEngine;

public interface IEyeParameters
{
    public IReactiveProperty<int> Mass { get; }
    public IReactiveProperty<float> Speed { get; }
    public float Force { get; }
    public Vector3 Position { get; }
    public ITransform EyeTransform { get; }
    public IReactiveProperty<bool> IsDeath { get; }
    public int ClanId { get; }
}