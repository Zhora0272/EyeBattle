using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _notification;
    [SerializeField] private CanvasGroup _notificationAlpha;
    [SerializeField] private CanvasGroup _notificationTextAlpha;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private Vector2 _sizeDelta;

    [SerializeField] private String[] messages;

    private ReactiveProperty<UIManager> _manager = new();
    private void Start()
    { 
        MainManager.WaitManager<UIManager>(manager =>
        {
            _manager.Value = (UIManager)manager;
        });

        _manager.Subscribe(manager =>
        {
            if(!manager) return;
            
            manager.SubscribeToPageActivate(UIPageType.TapToPlay, () =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                {
                    Activate(messages);
                    
                }).AddTo(this);
            });
            
        }).AddTo(this);
        
    }

    public void Activate(string message)
    {
        _notificationTextAlpha.DOFade(0, 0.5f);

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            _notification.text = message;
            ActivateNotification();

        }).AddTo(this);
        
    }

    public void Activate(string[] message, Action callBack = null)
    {
        if (Random.Range(0, 2) == 1)
        {
            Activate(message[Random.Range(0, message.Length)]);
            Observable.Timer(TimeSpan.FromSeconds(4)).Subscribe(_ => { Deactivate(); }).AddTo(this);
        }
    }

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
