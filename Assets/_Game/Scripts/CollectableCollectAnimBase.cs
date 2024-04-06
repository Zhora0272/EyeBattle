public abstract class CollectableCollectAnimBase
{
    public abstract void Activate(CachedMonoBehaviour mono, ITransform target, float duration);
    public abstract void Deactivate(CachedMonoBehaviour mono);
}