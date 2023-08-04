using UnityEngine;

namespace Vengadores.InjectionFramework
{
    /**
     * Helper MonoBehaviour for dynamically instantiated GameObjects.
     * If a prefab has AutoInjector on it; when you call GameObject.Instantiate(prefab),
     * it will automatically executes injection on every MonoBehavior including the children.
     */
    public class AutoInjector : MonoBehaviour
    {
        public void Awake()
        {
            ProjectContext.Instance.GetDiContainer().InjectAllMonoBehavioursOn(gameObject);
        }
    }
}
