using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Vengadores.ObjectPooling
{
    public class GameObjectPool : PoolBase<GameObject>
    {
        // Static fields
        private static Dictionary<GameObject, GameObjectPool> _poolsByPrefabs;
        private static Dictionary<GameObject, GameObjectPool> _poolsByGameObjectInstances;
        private static Transform _poolContainer;
        
        // Pool fields
        private readonly Stack<GameObject> _gameObjectPool = new Stack<GameObject>();
        private readonly GameObject _prefab;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadInstance()
        {
            _poolsByPrefabs = new Dictionary<GameObject, GameObjectPool>();
            _poolsByGameObjectInstances = new Dictionary<GameObject, GameObjectPool>();
        }

        [PublicAPI] public static GameObjectPool CreatePool(GameObject prefab)
        {
            // Create a global container for the objects in the stack
            if (_poolContainer == null)
            {
                _poolContainer = new GameObject("GameObjectPoolContainer").transform;
                Object.DontDestroyOnLoad(_poolContainer);
            }
            
            // Create the pool
            var pool = new GameObjectPool(prefab, _poolContainer);
            
            // Add to prefab lookup dict
            _poolsByPrefabs.Add(prefab, pool);
            
            // Add to instance lookup dict on instance create
            pool.OnInstanceCreated += (instance) =>
            {
                _poolsByGameObjectInstances.Add(instance, pool);
            };
            // Remove from instance lookup dict on instance dispose
            pool.OnInstanceDisposed += (instance) =>
            {
                _poolsByGameObjectInstances.Remove(instance);
            };
            // Remove from prefab lookup dict on pool dispose
            pool.OnPoolDisposed += () =>
            {
                _poolsByPrefabs.Remove(prefab);
            };
            
            return pool;
        }
        
        private GameObjectPool(GameObject prefab, Transform poolContainer)
        {
            _prefab = prefab;
            _poolContainer = poolContainer;
        }
        
        [PublicAPI] public void Allocate(int preAllocateCount)
        {
            for (var i = 0; i < preAllocateCount; i++)
            {
                CreateInstance();
            }
        }

        private void CreateInstance()
        {
            // Create instance in global pool container
            var go = Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity, _poolContainer);
            // Set name - remove (Clone)
            go.name = _prefab.name;
            // Disable the instance
            go.SetActive(false);
            
            // Add to stack
            _gameObjectPool.Push(go);
            
            RaiseOnInstanceCreated(go);
        }

        [PublicAPI] public GameObject Pop(Vector3 position, Quaternion rotation, Transform parent)
        {
            // If no instance in stack, create new one
            if (_gameObjectPool.Count == 0)
            {
                Allocate(1);
            }
            
            // Get instance from pool
            var instance = _gameObjectPool.Pop();
            instance.transform.SetParent(parent);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.transform.localScale = _prefab.transform.localScale;
            instance.SetActive(true);
            
            RaiseOnInstancePopped(instance);
            
            return instance;
        }

        [PublicAPI] public void Push(GameObject gameObject)
        {
            // Move instance to global pool container
            gameObject.transform.SetParent(_poolContainer);
            // Disable the instance
            gameObject.SetActive(false);
            // Add to stack
            _gameObjectPool.Push(gameObject);
            
            RaiseOnInstancePushed(gameObject);
        }

        [PublicAPI] public GameObject GetPrefab()
        {
            return _prefab;
        }

        [PublicAPI] public static GameObjectPool GetPoolOfPrefab(GameObject prefab)
        {
            if (_poolsByPrefabs.TryGetValue(prefab, out var pool))
            {
                return pool;
            }
            return null;
        }
        
        [PublicAPI] public static GameObjectPool GetPoolOfInstance(GameObject instance)
        {
            if (_poolsByGameObjectInstances.TryGetValue(instance, out var pool))
            {
                return pool;
            }
            return null;
        }
        
        [PublicAPI] public override void DisposePool()
        {
            // Pop and destroy instances
            while (_gameObjectPool.Count > 0)
            {
                var gameObject = _gameObjectPool.Pop();
                Object.Destroy(gameObject);
                RaiseOnInstanceDisposed(gameObject);
            }

            RaiseOnPoolDisposed();
        }
    }
}