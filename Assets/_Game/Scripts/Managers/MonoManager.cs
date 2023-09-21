using UnityEngine;

public abstract class MonoManager : MonoBehaviour
{
    protected virtual void Awake() =>
        MainManager.Register(this);

    protected virtual void OnDestroy() =>
        MainManager.UnRegister(this);
}