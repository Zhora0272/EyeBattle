using UniRx;
using UnityEngine;

public interface IScreenClickPoint
{
    public IReactiveProperty<Vector2> ScreenClickPointProperty { get; }
}