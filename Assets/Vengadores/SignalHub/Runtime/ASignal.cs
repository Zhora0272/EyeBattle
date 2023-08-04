using System;
using System.Collections.Generic;
using System.Data;
using JetBrains.Annotations;

namespace Vengadores.SignalHub
{
    public class SignalAlreadyAttachedException : Exception {}
    
    public abstract class ABaseSignal
    {
        public abstract void RemoveListener(object attachedObject);
    }
    
    public abstract class ASignal : ABaseSignal
    {
        private event Action Callback;
        private Dictionary<object, Action> _handlers = new Dictionary<object, Action>();

        [PublicAPI] public void AddListener(object attachedObject, Action handler)
        {
            if (attachedObject != null)
            {
                if (_handlers.ContainsKey(attachedObject))
                {
                    throw new SignalAlreadyAttachedException();
                }
                Callback += handler;
                _handlers[attachedObject] = handler;
                SignalHub.Attach(attachedObject, this);
            }
        }
        
        [PublicAPI] public override void RemoveListener(object attachedObject)
        {
            if (attachedObject != null && _handlers.TryGetValue(attachedObject, out var handler))
            {
                if (Callback != null) Callback -= handler;
                _handlers.Remove(attachedObject);
            }
        }

        [PublicAPI] public void Dispatch()
        {
            Callback?.Invoke();
        }
    }
    
    public abstract class ASignal<T> : ABaseSignal
    {
        private event Action<T> Callback;
        private Dictionary<object, Action<T>> _handlers = new Dictionary<object,Action<T>>();

        [PublicAPI] public void AddListener(object attachedObject, Action<T> handler)
        {
            if (attachedObject != null)
            {                
                if (_handlers.ContainsKey(attachedObject))
                {
                    throw new SignalAlreadyAttachedException();
                }
                Callback += handler;
                _handlers[attachedObject] = handler;
                SignalHub.Attach(attachedObject, this);
            }
        }
        
        [PublicAPI] public override void RemoveListener(object attachedObject)
        {
            if (attachedObject != null && _handlers.TryGetValue(attachedObject, out var handler))
            {
                if (Callback != null) Callback -= handler;
                _handlers.Remove(attachedObject);
            }
        }

        [PublicAPI] public void Dispatch(T arg)
        {
            Callback?.Invoke(arg);
        }
    }
    
    public abstract class ASignal<T1, T2> : ABaseSignal
    {
        private event Action<T1, T2> Callback;
        private Dictionary<object, Action<T1, T2>> _handlers = new Dictionary<object,Action<T1, T2>>();

        [PublicAPI] public void AddListener(object attachedObject, Action<T1, T2> handler)
        {
            
            if (attachedObject != null)
            {
                if (_handlers.ContainsKey(attachedObject))
                {
                    throw new SignalAlreadyAttachedException();
                }
                Callback += handler;
                _handlers[attachedObject] = handler;
                SignalHub.Attach(attachedObject, this);
            }
        }
        
        [PublicAPI] public override void RemoveListener(object attachedObject)
        {
            if (attachedObject != null && _handlers.TryGetValue(attachedObject, out var handler))
            {
                if (Callback != null) Callback -= handler;
                _handlers.Remove(attachedObject);
            }
        }
        
        [PublicAPI] public void Dispatch(T1 arg1, T2 arg2)
        {
            Callback?.Invoke(arg1, arg2);
        }
    }
    
    public abstract class ASignal<T1, T2, T3> : ABaseSignal
    {
        private event Action<T1, T2, T3> Callback;
        private Dictionary<object, Action<T1, T2,T3>> _handlers = new Dictionary<object,Action<T1, T2,T3>>();

        [PublicAPI] public void AddListener(object attachedObject, Action<T1, T2, T3> handler)
        {
            if (attachedObject != null)
            {
                if (_handlers.ContainsKey(attachedObject))
                {
                    throw new SignalAlreadyAttachedException();
                }
                Callback += handler;
                _handlers[attachedObject] = handler;
                SignalHub.Attach(attachedObject, this);
            }
        }
        
        [PublicAPI] public override void RemoveListener(object attachedObject)
        {
            if (attachedObject != null && _handlers.TryGetValue(attachedObject, out var handler))
            {
                if (Callback != null) Callback -= handler;
                _handlers.Remove(attachedObject);
            }
        }

        [PublicAPI] public void Dispatch(T1 arg1, T2 arg2, T3 arg3)
        {
            Callback?.Invoke(arg1, arg2, arg3);
        }
    }
}