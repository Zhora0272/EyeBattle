using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Smashlab.Pattern
{
    public class StateMachine : MonoBehaviour
    {
        private Dictionary<Type, BaseState> _availableStates;

        public BaseState CurrentState { get; private set; }
        public event Action<BaseState> OnStateChanged;

        public void SetStates(Dictionary<Type, BaseState> states)
        {
            _availableStates = states;
        }

        private void Update()
        {
            if (CurrentState == null)
            {
                CurrentState = _availableStates.Values.First();
            }

            Type nextState = CurrentState?.Tick();

            if (nextState != null && nextState != CurrentState?.GetType())
            {
                SwitchToNewState(nextState);
            }
        }

        public void SwitchToNewState(Type nextState)
        {
            CurrentState = _availableStates[nextState];
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}

