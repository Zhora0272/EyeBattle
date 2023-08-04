using UnityEngine;

namespace Smashlab.Pattern
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        private static bool _isQuitting = false;

        public static T instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogWarning("There is more than one manager of this type in this scene : " + typeof(T).Name);
                            return _instance;
                        }

                        if (_instance == null && !_isQuitting) return null;
                    }

                    return _instance;
                }
            }
        }

        public static void SetActive(bool active)
        {
            instance.gameObject.SetActive(active);
        }

        void OnDestroy()
        {
            OnDestroySpecific();
        }

        public virtual void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        protected virtual void OnDestroySpecific() { }
    }
}