using System;

namespace Vengadores.ObjectPooling
{
    public abstract class PoolBase<T>
    {
        public event Action<T> OnInstanceCreated;
        public event Action<T> OnInstancePopped;
        public event Action<T> OnInstancePushed;
        public event Action<T> OnInstanceDisposed;
        public event Action OnPoolDisposed;

        protected void RaiseOnInstanceCreated(T instance) { OnInstanceCreated?.Invoke(instance); }
        protected void RaiseOnInstancePopped(T instance) { OnInstancePopped?.Invoke(instance); }
        protected void RaiseOnInstancePushed(T instance) { OnInstancePushed?.Invoke(instance); }
        protected void RaiseOnInstanceDisposed(T instance) { OnInstanceDisposed?.Invoke(instance); }
        protected void RaiseOnPoolDisposed() { OnPoolDisposed?.Invoke(); }
        
        public abstract void DisposePool();
    }
}