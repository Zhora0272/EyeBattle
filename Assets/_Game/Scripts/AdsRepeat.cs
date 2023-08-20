using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AdsRepeat : MonoBehaviour
{
    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => 
        {
            

        }).AddTo(this);
    }
}
