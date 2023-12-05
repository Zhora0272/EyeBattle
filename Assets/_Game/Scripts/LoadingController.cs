using System;
using UniRx;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    private UIManager _uiManager;
    private IDisposable _disposable;
    private void Start()
    {
        _uiManager = MainManager.GetManager<UIManager>();
    }

    private void OnEnable()
    {
        _disposable?.Dispose();
        _disposable = Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
        {
            _uiManager.Activate(UIPageType.TapToPlay);       
        }).AddTo(this);
    }
}
