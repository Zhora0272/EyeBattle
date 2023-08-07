using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageOpenButton : MonoBehaviour
{
    [SerializeField] private UIPageType _pageType;
    private Button _button;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            MainManager.GetManager<UIManager>().Activte(_pageType);
        });
    }
}
