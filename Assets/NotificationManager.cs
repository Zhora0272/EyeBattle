using _Project.Scripts.Utilities;
using DG.Tweening;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _notification;
    [SerializeField] private CanvasGroup _notificationAlpha;
    [SerializeField] private CanvasGroup _notificationTextAlpha;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Vector2 _sizeDelta;

    public void Activate(string message)
    {
        _notification.text = message;
        ActivateNotification();
    }

    public void Activate(string[] message, Action callBack)
    {
        StartCoroutine(Helpers.RepeatWithDelayStringArgument(
            message.Length,
            1,
            Activate,
            message,
            () =>
        {
            Deactivate();
            callBack?.Invoke();
        }));
    }

    int index = 0;
    private void ActivateNotification()
    {
        _notificationAlpha.DOFade(1, 0.2f);
        _rectTransform.DOSizeDelta(_sizeDelta, 0.5f);
        _notificationTextAlpha.DOFade(1, 0.5f).SetEase(Ease.OutBack);
    }

    public void Deactivate()
    {
        _notificationAlpha.DOFade(1, 0.5f);
        _rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        _notificationTextAlpha.DOFade(0, 0.5f).SetEase(Ease.OutBack);
    }

}
