using System;
using TMPro;
using UniRx;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private IDisposable _timerDisposable;

    private int _timerValue;

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
        Application.targetFrameRate = 45;
    }
}