using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PageOpenButton : MonoBehaviour
{
    [SerializeField] private UIPageType _pageType;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if(_button == null)
        {
            Debug.LogError("button not found");
        }
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            MainManager.GetManager<UIManager>().Activate(_pageType);
        });
    }
}
