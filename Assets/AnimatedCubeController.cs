using UnityEngine;
using System;
using UniRx;
using DG.Tweening;
using Random = UnityEngine.Random;

public class AnimatedCubeController : MonoBehaviour
{
    private bool _inverseState;
    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(1f,2f))).Subscribe(_ =>
        {
            _inverseState = !_inverseState;

            if (_inverseState)
            {
                transform.DOMoveY(0, 0.8f);
            }
            else
            {
                transform.DOMoveY(0.15f, 0.8f);
            }

        }).AddTo(this);    
    }
}
