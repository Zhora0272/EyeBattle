using UniRx;
using UnityEngine;

public interface IEyeParameters
{
    public IReactiveProperty<int> Mass { get; }
    public IReactiveProperty<float> Speed { get; }
    public float Force { get; }
    public Vector3 Position { get; }
    public Transform EyeTransform { get; }
}