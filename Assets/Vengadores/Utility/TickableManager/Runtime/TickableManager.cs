using JetBrains.Annotations;
using UnityEngine;
using Vengadores.InjectionFramework;
using Vengadores.SignalHub;

namespace Vengadores.Utility.TickableManager
{
    public class TickSignal : ASignal { }
    public class LateTickSignal : ASignal { }
    
    public class TickableManager : MonoBehaviour, IInitializable
    {
        [Inject] private SignalHub.SignalHub _signalHub;

        private bool _isTicking;
        
        public void Init()
        {
            StartTicking();
        }

        [PublicAPI] public void StartTicking()
        {
            _isTicking = true;
        }

        [PublicAPI] public void StopTicking()
        {
            _isTicking = false;
        }

        [PublicAPI] public bool IsTicking()
        {
            return _isTicking;
        }
        
        private void Update()
        {
            if (_isTicking)
            {
                _signalHub.Get<TickSignal>().Dispatch();
            }
        }

        private void LateUpdate()
        {
            if (_isTicking)
            {
                _signalHub.Get<LateTickSignal>().Dispatch();
            }
        }
    }
}