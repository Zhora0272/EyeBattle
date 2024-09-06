using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

[Serializable] 
public class StringArray
{
    public string[] Mesages;
}

public class ShopIndicator : MonoBehaviour
{
    [SerializeField] private int _indicatorRepeatCount;
    [SerializeField] private float _indicatorRepeatInterval;
    [SerializeField] Vector2 _notificationShowTimeMinMax = new Vector2(40,120);

    [SerializeField] private NotificationManager _notificationManager;

    [SerializeField] private Vector2 _activateSize;
    [SerializeField] private Vector2 _deActivateSize;

    [SerializeField] private StringArray[] _messages;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Observable.Timer(TimeSpan.FromSeconds(Random.Range(
            _notificationShowTimeMinMax.x, _notificationShowTimeMinMax.y
            ))).Subscribe(_ => 
        {
            ActivateIndicator();

          /*  Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => 
            {
                StartCoroutine(Helpers.RepeatWithDelay(
                _indicatorRepeatCount,
                _indicatorRepeatInterval,
                Indicator));

            }).AddTo(this);*/

        }).AddTo(this);
    }

    private void ActivateIndicator()
    {
        _rectTransform.DOSizeDelta(_activateSize, 0.5f);

        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            //_notificationManager.Activate(_messages[0].Mesages, DeactivateIndicator);

        }).AddTo(this);
    }
}
