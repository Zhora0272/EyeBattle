using System.Collections.Generic;
using _Game.Scripts.UI;
using UnityEngine;
using System;

public class UIManager : MonoManager
{
    [SerializeField] private UIPageType _defaultStartPage = UIPageType.TapToPlay;
    [SerializeField] private UIPage[] _pages;
    [SerializeField] private UISubPage[] _subPages;

    private Dictionary<UIPageType, List<Action>> _subscribeActivateEvents;
    private Dictionary<UIPageType, List<Action>> _subscribeDeactivateEvents;

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
                    foreach (var action in result)
                    {
                        action?.Invoke();
                    }
                }

                page.Activate();
            }
            else
            {
                if (page.gameObject.activeInHierarchy)
                {
                    if (_subscribeDeactivateEvents.TryGetValue(page.PageTye, out var result))
                    {
                        foreach (var action in result)
                        {
                            action?.Invoke();
                        }
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
        AddActionToDictionary(_subscribeActivateEvents, pageType, subject);
    }

    public void SubscribeToPageDeactivate
    (
        UIPageType pageType,
        Action subject
    )
    {
        AddActionToDictionary(_subscribeDeactivateEvents, pageType, subject);
    }

    private void AddActionToDictionary(Dictionary<UIPageType, List<Action>> dictionary, UIPageType pageType, Action subject)
    {
        if (dictionary.TryGetValue(pageType, out var value))
        {
            value.Add(subject);
            dictionary.TryAdd(pageType, value);
        }
        else
        {
            dictionary.TryAdd(pageType, new List<Action>() {subject});
        }
    }
}