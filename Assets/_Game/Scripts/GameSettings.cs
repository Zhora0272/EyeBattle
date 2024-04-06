using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private RenderTexture _renderTexture;
    [SerializeField] private AspectRatioFitter _ratioFitter;
    [SerializeField, Range(0.1f, 1)] private float _qualityCoeficient = .85f;

    private IDisposable _timerDisposable;

    private int _timerValue;

    private int _newWidth = Screen.width;
    private int _newHeight = Screen.height;

    private void Awake()
    {
        ChangeResolution
        (
            (int)(_newWidth * _qualityCoeficient),
            (int)(_newHeight * _qualityCoeficient)
        );
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

        _ratioFitter.aspectRatio = ((float)width * 1.1f/ (float)height);
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