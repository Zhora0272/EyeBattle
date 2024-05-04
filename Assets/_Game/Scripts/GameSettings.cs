using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField, Range(0.1f, 1)] private float _qualityCoeficient = .85f;

    private IDisposable _timerDisposable;

    private int _timerValue;

    private int _newWidth;
    private int _newHeight;

    private void OnEnable()
    {
        var res = new Vector2(Screen.width, Screen.height);
        ChangeResolution
        (
            (int)(res.x * _qualityCoeficient),
            (int)(res.y * _qualityCoeficient)
        );
        
        print(res);
    }

    private void ChangeResolution(int width, int height)
    {
        if (_renderTexture != null)
        {
            _renderTexture.Release();
            _renderTexture.width = width;
            _renderTexture.height = height;
            _renderTexture.Create();
        }
    }

    private void Start()
    {
        ApplicationSettings();

        MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, GameStartDisposableEvents);
        MainManager.GetManager<UIManager>()
            .SubscribeToPageDeactivate(UIPageType.InGame, () => { _timerDisposable?.Dispose(); });
    }

    private void GameStartDisposableEvents()
    {
        StartTimer();
    }

    private void StartTimer()
    {
        var startTime = DateTime.Now;

        _timerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            var calculationTime = DateTime.Now - startTime;

            var dateTime = new DateTime(
                startTime.Year,
                startTime.Month,
                
                startTime.Day,
                startTime.Hour,
                calculationTime.Minutes,
                calculationTime.Seconds);

            _timerText.text = dateTime.ToString("mm:ss");
        }).AddTo(this);
    }

    private void ApplicationSettings()
    {
        Application.targetFrameRate = 60;
    }
}