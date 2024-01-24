using DG.Tweening;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class BrokenEyeCollection : CachedMonoBehaviour
{
    [SerializeField] private CanvasGroup _loadBarCanvasGroup;
    [SerializeField] private TriggerCheckController _triggerCheckController;

    public IObservable<float> BrokenPartsCollectionStream => _brokenPartCollectionSubject;
    private Subject<float> _brokenPartCollectionSubject = new();

    private IDisposable _indicatorHideDisposable;

    private void Awake()
    {
        _triggerCheckController.TriggerLayerEnterRegister(Layer.Eye, BrokenEyeEnterTrigger);
    }

    private void BrokenEyeEnterTrigger(Collider other)
    {
        _indicatorHideDisposable?.Dispose();

        _loadBarCanvasGroup.DOKill();
        _loadBarCanvasGroup.DOFade(1, 1).SetEase(Ease.OutBack);

        if (other.TryGetComponent<Collectable>(out var result))
        {
            if (result.CollectState) return;

            if (Time.time - result.BrokenTime < 1)
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                    {
                        Collect(result, other, 1);
                    })
                    .AddTo(this);
            }
            else
            {
                Collect(result, other, 0.3f);
            }
        }
    }

    private void Collect(Collectable result, Collider other, float duration)
    {
        float value = 0;

        Debug.Log(name);
        value = result.Collect(this, duration);

        //there is a animation when broken element will be UP
        /*other.transform.DOMove(other.transform.position + Vector3.up * 3, duration).onComplete = () =>
        {
          
            _brokenPartCollectionSubject.OnNext(value);
        };*/

        _indicatorHideDisposable = Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
        {
            _loadBarCanvasGroup.DOFade(0, 1).SetEase(Ease.OutBack);
        }).AddTo(this);
    }
}