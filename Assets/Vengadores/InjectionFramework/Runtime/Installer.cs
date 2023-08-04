using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Vengadores.Utility.LogWrapper;

namespace Vengadores.InjectionFramework
{
    /**
     * It will create instances, binds them to the DiContainer and injects objects to all instances.
     * Order of execution:
     * 1- Install "ProjectInstaller" if it is not installed already
     * 2- Create instances defined in Setup() implementation and bind them to DiContainer
     * 3- Search the scene for "Binding" components and bind them to DiContainer (Skipped for project installer)
     * 4- All instances are ready, start injecting to fields defined with [Inject] attribute
     */
    public abstract class Installer : MonoBehaviour
    {
        private DiContainer _diContainer;
        
        private Queue<ContainerRegistry> _injectionQueue;
        private List<ContainerRegistry> _registryList;

        private void Awake()
        {
            Profiler.BeginSample("**[InjectionFramework] Installer " + gameObject.name);
            
            gameObject.name = "#" + GetType().Name + "#";

            ProjectContext.Instance.TryInit();

            _diContainer = ProjectContext.Instance.GetDiContainer();
            
            _injectionQueue = new Queue<ContainerRegistry>();
            _registryList = new List<ContainerRegistry>();
            
            Setup();
            SetupSceneObjects();
            
            GameLog.Log(
                "Injection", 
                GameLog.GetColoredText(Color.yellow, GetType().Name) + " setup complete in " + GameLog.GetColoredText(Color.cyan, gameObject.scene.name), 
                this);
            
            InjectQueue();
            
            GameLog.Log(
                "Injection", 
                GameLog.GetColoredText(Color.green, GetType().Name) + " injections complete", 
                this);

            PostSetup();
            
            Profiler.EndSample();
        }

        private void Start()
        {
            // Just to find installer in hierarchy easier
            transform.SetAsFirstSibling();
        }

        protected abstract void Setup();
        protected virtual void PostSetup() { }
        protected virtual void PreDestroy() { }

        private void SetupSceneObjects()
        {
            if(gameObject.scene.name == "DontDestroyOnLoad") return;
            
            var rootGameObjects =  
                gameObject.scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<Installer>() == null);

            foreach (var root in rootGameObjects)
            {
                // Get binding components on itself or its any children, bind attached components
                var bindComponents = root.GetComponentsInChildren<Binding>(true);
                foreach (var bindComponent in bindComponents)
                {
                    foreach (var bindableComponent in bindComponent.Components)
                    {
                        BindObject(bindableComponent, bindComponent.Id);
                    }
                }

                // Get mono behaviours on itself or its any children, add to injection queue
                foreach (var monoBehaviour in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    _injectionQueue.Enqueue(new ContainerRegistry(monoBehaviour));
                }
            }
        }

        private void InjectQueue()
        {
            var injectedObjects = new HashSet<object>();
            
            while (_injectionQueue.Count > 0)
            {
                var registry = _injectionQueue.Dequeue();
                _diContainer.Inject(registry.Object);
                injectedObjects.Add(registry.Object);
            }
        
            foreach (var injectedObject in injectedObjects)
            {
                if (injectedObject is IInitializable initializableObj)
                {
                    initializableObj.Init();
                }
            }
        }
    
        private void OnDestroy()
        {
            PreDestroy();
            
            foreach (var registry in _registryList)
            {
                _diContainer.UnregisterInstance(registry);
                
                if (registry.Object is IDisposable disposableObj)
                {
                    disposableObj.Dispose();
                }
            }
            
            GameLog.Log(
            "Injection", 
                GameLog.GetColoredText(new Color(0f, 0.5f, 0.5f), GetType().Name) + " disposed");
        }

        // Helpers
        protected void BindObject(object obj, string id = null)
        {
            var registry = new ContainerRegistry(obj, id);
            _diContainer.RegisterInstance(registry);
            _registryList.Add(registry);
            _injectionQueue.Enqueue(registry);
        }

        protected T FromNew<T>(string id = null)
        {
            var obj = Activator.CreateInstance<T>();
            BindObject(obj, id);
            return obj;
        }

        protected T FromNewComponent<T>(string id = null) where T : Component
        {
            var component = gameObject.AddComponent<T>();
            BindObject(component, id);
            return component;
        }

        protected T GetBinding<T>(string id = null)
        {
            return _diContainer.Get<T>(id);
        }
        
        [ContextMenu("Print Registered Types")]
        public void PrintRegisteredTypes()
        {
            var result = "";
            foreach (var registry in _registryList)
            {
                result += registry.Type + "\n";
            }
            GameLog.Log(result);
        }
        
        [ContextMenu("Print All Types in Container")]
        public void PrintContainer()
        {
            var result = "";
            foreach (var obj in ProjectContext.Instance.GetDiContainer().GetAllInstances())
            {
                result += obj.GetType() + "\n";
            }
            GameLog.Log(result);
        }
    }
}
