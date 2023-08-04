using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Vengadores.SignalHub
{
    public class SignalHub
    {
        private static Dictionary<object, List<ABaseSignal>> _signalsByObject;
        
        private readonly Dictionary<Type, ABaseSignal> _signalsByType;

        [PublicAPI] public SignalHub()
        {
            _signalsByObject = new Dictionary<object, List<ABaseSignal>>();
            _signalsByType = new Dictionary<Type, ABaseSignal>();
        }
        
        [PublicAPI] public T Get<T>() where T : ABaseSignal, new()
        {
            var signalType = typeof(T);

            if (_signalsByType.TryGetValue(signalType, out var signal))
            {
                return ((T) signal);
            }
            
            signal = (ABaseSignal) Activator.CreateInstance(signalType);
            _signalsByType.Add(signalType, signal);

            return (T) signal;
        }

        [PublicAPI] public void RemoveAllListeners(object attachedObject)
        {
            if (_signalsByObject.TryGetValue(attachedObject, out var signalsToRemove)) 
  			{
                foreach (var signal in signalsToRemove)
                {
                    signal.RemoveListener(attachedObject);
                }
                _signalsByObject[attachedObject].Clear();
            }
        }
        
        internal static void Attach(object attachedObject, ABaseSignal signalObject)
        {
            if (!_signalsByObject.ContainsKey(attachedObject))
            {
                _signalsByObject.Add(attachedObject, new List<ABaseSignal>());
            }
            _signalsByObject[attachedObject].Add(signalObject);
        }
    }
}