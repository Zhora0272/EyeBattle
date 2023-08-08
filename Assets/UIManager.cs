using UnityEngine;

public class UIManager : MonoManager
{
    [SerializeField] private UIPage[] _pages;

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
                page.Activate();
            }
            else
            {
                page.Deactivate();
            }
        }
    }
}

public enum UIPageType
{
    Empty,
    Shop,
    Donate,
}
