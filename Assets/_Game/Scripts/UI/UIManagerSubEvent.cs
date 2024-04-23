using System;
using UnityEngine;

public class UIManagerSubEvent : MonoBehaviour
{
    public enum EventType
    {
        Enable,
        Disable,
        Delete
    }

    

    [Serializable]
    public class UISubEvent
    {
        public EventType eventType;
        public UIPageType pageType;
    }

    [SerializeField] private UISubEvent[] _subEvents;
    
    private void Start()
    {
        foreach (var item in _subEvents)
        {
            MainManager.GetManager<UIManager>().SubscribeToPageActivate(item.pageType, () =>
            {
                switch (item.eventType)
                {
                    case EventType.Enable:
                    {
                        gameObject.SetActive(true);
                        break;
                    }
                    case EventType.Disable:
                    {
                        gameObject.SetActive(false);
                        break;
                    }
                    case EventType.Delete : break;
                }
            });
        }
    }
}