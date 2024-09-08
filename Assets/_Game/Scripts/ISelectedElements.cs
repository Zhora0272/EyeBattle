using UniRx;
using UnityEngine;

public interface ISelectedElements
{
    public ReactiveProperty<Collider[]> HitColliders { get; }
}