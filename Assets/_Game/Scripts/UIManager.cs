using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoManager
{
    [SerializeField] private UIPage[] _pages;

    private Dictionary<UIPageType, Action> _subscribeActivateEvents;
    private Dictionary<UIPageType, Action> _subscribeDeactivateEvents;

    protected override void Awake()
    {
        base.Awake();
        _subscribeActivateEvents = new();
        _subscribeDeactivateEvents = new();
    }

    private void Start()
    {
        _pages = GetComponentsInChildren<UIPage>(true);
    }

    public void Activte(UIPageType pageType)
    {
        foreach (var page in _pages)
        {
            if(page.PageTye == pageType)
            {
                if (_subscribeActivateEvents.TryGetValue(page.PageTye, out var result))
                {
                    result?.Invoke();
                }
                page.Activate();
            }
            else
            {
                if(page.gameObject.activeInHierarchy)
                {
                    if(_subscribeDeactivateEvents.TryGetValue(page.PageTye, out var result))
                    {
                        result?.Invoke();
                    }
                }
                page.Deactivate();
            }
        }
    }

    public void SubscribeToPageActivate
    (
       UIPageType layer,
       Action subject
    )
    {
        if (_subscribeActivateEvents.TryGetValue(layer, out var value))
        {
            value += subject;
            _subscribeActivateEvents.TryAdd(layer, value);
        }
        else
        {
            _subscribeActivateEvents.TryAdd(layer, subject);
        }
    }

    public void SubscribeToPageDeactivate
    (
       UIPageType layer,
       Action subject
    )
    {
        if (_subscribeDeactivateEvents.TryGetValue(layer, out var value))
        {
            value += subject;
            _subscribeDeactivateEvents.TryAdd(layer, value);
        }
        else
        {
            _subscribeDeactivateEvents.TryAdd(layer, subject);
        }
    }
}

public enum UIPageType
{
    Empty,
    Shop,
    Donate,
    TapToPlay,
    InGame
}
