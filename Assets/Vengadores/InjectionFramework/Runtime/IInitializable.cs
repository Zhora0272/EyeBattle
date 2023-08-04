namespace Vengadores.InjectionFramework
{
    /**
     * Optional interface for the objects.
     * It will receive Init() call when the installer finishes all the injections.
     * Install phase is done in Awake(), so in Start() method we know that all injections are ready.
     * However, it is useful for non-MonoBehavior objects since they don't have Start().
     */
    public interface IInitializable
    {
        void Init();
    }
}