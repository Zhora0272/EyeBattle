using System;
using System.Collections.Generic;
using _Game.Scripts.UI;
using UnityEngine;

public class UIManager : MonoManager
{
    [SerializeField] private UIPageType _defaultStartPage = UIPageType.TapToPlay;
    [SerializeField] private UIPage[] _pages;
    [SerializeField] private UISubPage[] _subPages;

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
        _subPages = GetComponentsInChildren<UISubPage>(true);
        
        Activate(_defaultStartPage);
        Activate(UISubPageType.Empty);
    }

    public void Activate(UISubPageType subPageType)
    {
        foreach (var subPage in _subPages)
        {
            if (subPage.PageTye == subPageType)
            {
                subPage.Activate();
            }
            else
            {
                subPage.Deactivate();
            }
        }
    }

    public void Activate(UIPageType pageType)
    {
        foreach (var page in _pages)
        {
            if (page.PageTye == pageType)
            {
                if (_subscribeActivateEvents.TryGetValue(page.PageTye, out var result))
                {
                    result?.Invoke();
                }

                page.Activate();
            }
            else
            {
                if (page.gameObject.activeInHierarchy)
                {
                    if (_subscribeDeactivateEvents.TryGetValue(page.PageTye, out var result))
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
        UIPageType pageType,
        Action subject
    )
    {
        if (_subscribeActivateEvents.TryGetValue(pageType, out var value))
        {
            value += subject;
            _subscribeActivateEvents.TryAdd(pageType, value);
        }
        else
        {
            _subscribeActivateEvents.TryAdd(pageType, subject);
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