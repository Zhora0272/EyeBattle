using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Vengadores.InjectionFramework.Tests.PlayModeTests
{
    public class InstallerTest
    {
        private DiContainer _diContainer;

        private InjectableMonoOnScene _componentOnRoot;
        private InjectableMonoOnScene _componentOnAnotherRoot;
        private InjectableMonoOnScene _componentOnSubChild;

        private GameObject _subChildOther;
        
        [SetUp]
        public void SetUp()
        {
            // Fake ProjectInstaller initialization using Reflection
            var injectionFrameworkAssembly = Assembly.GetAssembly(typeof(Installer));
            var projectContextType = injectionFrameworkAssembly.GetType("Vengadores.InjectionFramework.ProjectContext");
            
            // Get ProjectContext.Instance
            var instanceProperty = projectContextType.GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic);
            var instance = instanceProperty?.GetValue(null);
            
            // Set _isInitialize true to fake init
            var field = projectContextType.GetField("_isInitialized", BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(instance, true);
            
            // Get DiContainer for testing purposes
            var diContainerField = projectContextType.GetField("_diContainer", BindingFlags.Instance | BindingFlags.NonPublic);
            _diContainer = (DiContainer)diContainerField?.GetValue(instance);
            
            // Add test objects to the scene
            var root = new GameObject("Root");
            var anotherRoot = new GameObject("AnotherRoot");
            var child = new GameObject("Child");
            var subChild = new GameObject("SubChild");
            _subChildOther = new GameObject("SubChildOther");
            
            // Create a set of children to reproduce recursions
            child.transform.SetParent(root.transform);
            subChild.transform.SetParent(child.transform);
            _subChildOther.transform.SetParent(child.transform);

            // Attach injectable components
            _componentOnRoot = root.AddComponent<InjectableMonoOnScene>();
            _componentOnAnotherRoot = anotherRoot.AddComponent<InjectableMonoOnScene>();
            _componentOnSubChild = subChild.AddComponent<InjectableMonoOnScene>();

            // Attach binding components
            var bindedMonoOnScene = root.AddComponent<BindedMonoOnScene>();
            var binding = root.AddComponent<Binding>();
            binding.Id = "TestId";
            binding.Components = new List<Component> {bindedMonoOnScene};
        }

        [UnityTest]
        public IEnumerator TestSetupSceneObjects()
        {
            // Create installer
            var installerObj = new GameObject("InstallerObj", typeof(SimpleInstaller));
            var installer = installerObj.GetComponent<SimpleInstaller>();
            yield return null;

            // Injections on fields with [Inject] attributes
            Assert.AreEqual(_componentOnRoot.GetParam(), "TestParam");
            Assert.AreEqual(_componentOnAnotherRoot.GetParam(), "TestParam");
            Assert.AreEqual(_componentOnSubChild.GetParam(), "TestParam");
            
            // Installed mono
            Assert.DoesNotThrow(()=>
            {
                // Get installed instance
                var anotherInjectableMono = _diContainer.Get<AnotherInjectableMono>();
                // Injections on installed instance should be ready 
                Assert.AreEqual(anotherInjectableMono.GetParam(), 5);
                // Init should be called for IInitializables
                Assert.AreEqual(anotherInjectableMono.isInitCalled, true);
                Assert.AreEqual(anotherInjectableMono.isDisposeCalled, false);
            });
            
            // Installed plain object
            Assert.DoesNotThrow(()=>
            {
                Assert.NotNull(_diContainer.Get<PlainClass>());
            });
            
            // Auto binding
            Assert.NotNull(_diContainer.Get<BindedMonoOnScene>("TestId"));

            // DiContainer runtime creation helpers
            Assert.NotNull(_diContainer.CreateFromNewComponent<Canvas>(_componentOnRoot.gameObject));
            Assert.NotNull(_diContainer.CreateFromNew<PlainClass>());
            
            // Get the installed test mono instance
            var anotherInjectableMono = _diContainer.Get<AnotherInjectableMono>();
            
            // AutoInject
            var autoInjectableMono = _subChildOther.AddComponent<AnotherInjectableMono>();
            _subChildOther.AddComponent<AutoInjector>();
            Assert.AreEqual(autoInjectableMono.GetParam(), 5);
            
            // Trigger debug helpers to print contents
            installer.PrintContainer();
            installer.PrintRegisteredTypes();
            
            // Destroy installer to simulate scene unload
            // ---------------------------------------------
            Object.Destroy(installer.gameObject);
            // ---------------------------------------------
            
            // Wait for destroy calls
            yield return null;
            
            // Installed instances should be removed from the container
            Assert.Throws<NullReferenceException>(() => { _diContainer.Get<AnotherInjectableMono>(); });
            
            // Dispose should be called for IDisposables
            Assert.AreEqual(anotherInjectableMono.isDisposeCalled, true);
            
            // Trigger debug helpers to print contents
            installer.PrintContainer();
            installer.PrintRegisteredTypes();
        }
    }
    
    public class PlainClass { }
    
    public class BindedMonoOnScene : MonoBehaviour { }

    public class InjectableMonoOnScene : MonoBehaviour
    {
        [Inject] private string _param;

        public string GetParam() { return _param; }
    }
    
    public class AnotherInjectableMono : MonoBehaviour, IInitializable, IDisposable
    {
        [Inject] private int _param;

        public bool isInitCalled;
        public bool isDisposeCalled;
        
        public int GetParam() { return _param; }
        
        public void Init()
        {
            isInitCalled = true;
        }

        public void Dispose()
        {
            isDisposeCalled = true;
        }
    }

    public class SimpleInstaller : Installer
    {
        protected override void Setup()
        {
            BindObject("TestParam");
            BindObject(5);

            FromNewComponent<AnotherInjectableMono>();
            FromNew<PlainClass>();
        }
    }
}