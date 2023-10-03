using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class BrokenEyeCollection : CachedMonoBehaviour
{
    [SerializeField] private CanvasGroup _loadBarCanvasGroup;
    
    public IObservable<float> BrokenPartsCollectionStream => _breokenPartCollectionSubject;
    private Subject<float> _breokenPartCollectionSubject = new();

    private IDisposable _updateDisposable;

    private IDisposable _indicatorHideDisposable;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("BrokenEye"))
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
                    }).AddTo(this);
                }
                else
                {
                    Collect(result, other, 0.3f);
                }
            }
        }
    }

    private void Collect(Collectable result, Collider other, float duration)
    {
        float value = 0;

        other.transform.DOMove(other.transform.position + Vector3.up * 3, duration).onComplete = () =>
        {
            value = result.Collect(transform.position);

            var eyeTransform = other.transform;

            _breokenPartCollectionSubject.OnNext(value);

            _updateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                eyeTransform.position = Vector3.Lerp(eyeTransform.position,
                    transform.position,
                    Time.deltaTime * 10);

            }).AddTo(result);
        };

        _indicatorHideDisposable = Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
        {
            _loadBarCanvasGroup.DOFade(0, 1).SetEase(Ease.OutBack);
        }).AddTo(this);
    }

    private void OnDisable()
    {
        _updateDisposable?.Dispose();
    }
}
