using System;
using UnityEngine;

public class UIManagerSubEvent : MonoBehaviour
{
    public enum EventType
    {
        Enable,
        Disable,
        Delete,
        EnableDisable,
        DisableEnable
    }

    [Serializable]
    public class UISubEvent
    {
        public UIPageType pageType;
        public EventType eventType;
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
            /*MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(item.pageType, () =>
            {
                switch (item.eventType)
                {
                    case EventType.Enable:
                    {
                        gameObject.SetActive(false);
                        break;
                    }
                    case EventType.Disable:
                    {
                        gameObject.SetActive(true);
                        break;
                    }
                    case EventType.Delete : break;
                }
            });*/
        }
    }
}