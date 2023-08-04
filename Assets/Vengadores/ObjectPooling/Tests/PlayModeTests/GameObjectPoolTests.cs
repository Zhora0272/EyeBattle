using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Vengadores.ObjectPooling.Tests.PlayModeTests
{
    public class GameObjectPoolTests
    {
        private GameObject _prefab;
        private GameObject _prefabOther;
        private Transform _root;
        
        [SetUp]
        public void Prepare()
        {
            _prefab = new GameObject("TestPrefab",typeof(TestComponent));
            _prefabOther = new GameObject("TestPrefabOther");
            _root = new GameObject("Root").transform;
        }

        [Test]
        public void GameObjectPoolTest()
        {
            var testPool = GameObjectPool.CreatePool(_prefab.gameObject);
            
            Assert.NotNull(testPool.GetPrefab());
            
            Assert.Null(GameObjectPool.GetPoolOfPrefab(_prefabOther));
            Assert.NotNull(GameObjectPool.GetPoolOfPrefab(_prefab));
            
            var createdCount = 0;
            testPool.OnInstanceCreated += (o) => { createdCount++; };
            
            var poppedCount = 0;
            testPool.OnInstancePopped += (o) => { poppedCount++; };
            
            var pushedCount = 0;
            testPool.OnInstancePushed += (o) => { pushedCount++; };
            
            var disposedCount = 0;
            testPool.OnInstanceDisposed += (o) => { disposedCount++; };
            
            // Allocate
            testPool.Allocate(2);
            Assert.AreEqual(createdCount, 2);
            
            // Pop
            var poppedInstance = testPool.Pop(Vector3.zero, Quaternion.identity, _root);
            Assert.NotNull(poppedInstance);
            Assert.AreEqual(poppedCount, 1);
            testPool.Pop(Vector3.zero, Quaternion.identity, _root);
            Assert.AreEqual(poppedCount, 2);
            
            Assert.NotNull(GameObjectPool.GetPoolOfInstance(poppedInstance));
            Assert.Null(GameObjectPool.GetPoolOfInstance(new GameObject()));
            
            // Empty
            var newInstance = testPool.Pop(Vector3.zero, Quaternion.identity, _root); // will allocate new
            Assert.AreEqual(createdCount, 3);
            Assert.AreEqual(poppedCount, 3);
            
            // Push
            testPool.Push(poppedInstance);
            testPool.Push(newInstance);
            Assert.AreEqual(pushedCount, 2);
            
            testPool.DisposePool();
            Assert.AreEqual(disposedCount, 2);
        }

        [UnityTest]
        public IEnumerator PushToPoolAfterDelayTest()
        {
            var testPool = GameObjectPool.CreatePool(_prefab.gameObject);
            testPool.OnInstanceCreated += (instance) =>
            {
                var component = instance.AddComponent<PushToPoolAfterDelay>();
                component.DespawnAfter = 0.25f;
            };
            bool isPushed = false;
            testPool.OnInstancePushed += (instance) =>
            {
                isPushed = true;
            };
            var gameObject = testPool.Pop(Vector3.zero, Quaternion.identity, _root);
            Assert.AreEqual(isPushed, false);

            yield return new WaitForSeconds(0.5f);
            
            Assert.AreEqual(isPushed, true);
            
            testPool.DisposePool();
        }
        
        [TearDown]
        public void Dispose()
        {
            Object.Destroy(_prefab);
            Object.Destroy(_prefabOther);
            Object.Destroy(_root.gameObject);
        }
    }
    
    public class TestComponent : MonoBehaviour { }
}