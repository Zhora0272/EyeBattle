using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vengadores.ObjectPooling
{
    public class ObjectPool<T> : PoolBase<T> where T : class
    {
        private readonly Stack<T> _objectPool = new Stack<T>();

        [PublicAPI] public static ObjectPool<T> CreatePool()
        {
            return new ObjectPool<T>();
        }

        private ObjectPool() { }
        
        [PublicAPI] public void Allocate(int allocateCount, params object[] constructorArgs)
        {
            for (var i = 0; i < allocateCount; i++)
            {
                CreateInstance(constructorArgs);
            }
        }

        private T CreateInstance(params object[] constructorArgs)
        {
            T instance;
            if (constructorArgs == null || constructorArgs.Length == 0)
            {
                instance = Activator.CreateInstance<T>();
            }
            else
            {
                instance = (T) Activator.CreateInstance(typeof(T), constructorArgs);
            }
            
            _objectPool.Push(instance);
            
            RaiseOnInstanceCreated(instance);
            
            return instance;
        }
        
        [PublicAPI] public T Pop(params object[] constructorArgs)
        {
            if (_objectPool.Count == 0)
            {
                Allocate(1, constructorArgs);
            }

            var instance = _objectPool.Pop();
            
            RaiseOnInstancePopped(instance);
            
            return instance;
        }

        [PublicAPI] public void Push(T obj)
        {
            _objectPool.Push(obj);
            
            RaiseOnInstancePushed(obj);
        }
        
        [PublicAPI] public override void DisposePool()
        {
            while (_objectPool.Count > 0)
            {
                var instance = _objectPool.Pop();
                RaiseOnInstanceDisposed(instance);
            }
            
            RaiseOnPoolDisposed();
        }
    }
}