using UnityEngine;
using Vengadores.Utility.LogWrapper;
using Object = UnityEngine.Object;

namespace Vengadores.InjectionFramework
{
    /**
     * Global instance for DiContainer and ProjectInstaller
     */
    internal class ProjectContext
    {
        internal static ProjectContext Instance { get; private set; }
        
        private const string ProjectInstallerFilename = "ProjectInstaller";
        
        private readonly DiContainer _diContainer = new DiContainer();
        
        private bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadInstance()
        {
            Instance = new ProjectContext();
        }
        
        public void TryInit()
        {
            if (_isInitialized) return;
            
            _isInitialized = true;

            var prefab = GetProjectInstallerPrefab();
            
            if (prefab != null)
            {
                // To create project installer on dontDestroyOnLoad scene
                var dontDestroyOnLoadObj = new GameObject();
                Object.DontDestroyOnLoad(dontDestroyOnLoadObj);

                // Now in installer awake func, scene is dontDestroyOnLoad
                var installerObject = Object.Instantiate(prefab, dontDestroyOnLoadObj.transform);
                installerObject.transform.SetParent(null, false);
                Object.DontDestroyOnLoad(installerObject);
                
                // Remove the temp obj
                Object.Destroy(dontDestroyOnLoadObj);
            }
            else
            {
                GameLog.Log(
                    "Injection", 
                    GameLog.GetColoredText(Color.red, "ProjectInstaller") + " not found in Resources");
            }
        }

        private static GameObject GetProjectInstallerPrefab()
        {
            var prefab = Resources.Load(ProjectInstallerFilename, typeof(GameObject));
            return prefab == null ? null : (GameObject)prefab;
        }

        public DiContainer GetDiContainer()
        {
            return _diContainer;
        }
    }
}