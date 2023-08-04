using System;
using UnityEngine;
using Vengadores.InjectionFramework;

namespace Vengadores.UIFramework.ScreenQueueManager
{
    public class ScreenTask<TScreen> : IQueueableTask 
        where TScreen : UIScreenBase
    {
        [Inject] private UIFrame _uiFrame;
        private readonly IScreenProperties _properties;
        
        public event Action<IQueueableTask> OnComplete;
        public int Priority { get; }
        
        public ScreenTask(int priority = 1, IScreenProperties properties = null, UIFrame frame = null)
        {
            _properties = properties;
            Priority = priority;
            if (_uiFrame == null && frame != null)
            {
                _uiFrame = frame;
            }
        }

        public void Execute()
        {
            _uiFrame.AddEventForAllScreens(OnScreenEvent.Closed, OnScreenClosed);
            _uiFrame.Open<TScreen>();
        }

        private void OnScreenClosed(UIScreenBase screen)
        {
            if (screen.GetType() == typeof(TScreen))
            {
                _uiFrame.RemoveEventForAllScreens(OnScreenEvent.Closed, OnScreenClosed);
                OnComplete?.Invoke(this);
            }
        }
    }
}
