using UnityEngine;
using System;
using UniRx;

public class AnimationRepeat : MonoBehaviour
{
    private Animation _animation;
    private IDisposable _animationCheckDisposable;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
    }

    private void OnEnable()
    {
        _animationCheckDisposable = Observable.Interval(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
        {
            if (!_animation.isPlaying)
            {
                _animation.Play();
            }
        }).AddTo(this);
    }

    private void OnDisable()
    {
        _animationCheckDisposable.Dispose();
    }
}