namespace Vengadores.InjectionFramework
{
    /**
     * Optional interface for the objects.
     * It will receive Dispose() call when the installer destroyed (Scene unload etc.)
     * It is useful for non-MonoBehavior objects since they don't have OnDestroy().
     */
    public interface IDisposable
    {
        void Dispose();
    }
}