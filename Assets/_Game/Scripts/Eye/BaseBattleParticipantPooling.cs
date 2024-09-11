using UnityEngine;
using Zenject;

public class BaseBattleParticipantPooling<T> : MonoMemoryPool<T> where T : MoveableBattleParticipantBaseController
{
    protected override void OnCreated(T item)
    {
        base.OnCreated(item);
        Debug.Log("Object Created");
    }

    protected override void OnSpawned(T item)
    {
        base.OnSpawned(item);
        item.OnSpawned();
    }

    protected override void OnDespawned(T item)
    {
        base.OnDespawned(item);
        item.OnDespawned();
    }
}