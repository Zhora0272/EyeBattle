using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vengadores.Utility.UICrawler
{
    public class UICrawlerTouchIndicator : MonoBehaviour
    {
        private static readonly float Duration = 0.2f;
        
        public Image image;

        private Sequence _sequence;

        private void Start()
        {
            transform.localScale = Vector3.one * 0.4f;
            _sequence = DOTween.Sequence();
            _sequence.SetUpdate(UpdateType.Normal, true);
            _sequence.Append(transform.DOScale(1, Duration).SetEase(Ease.OutSine));
            _sequence.Append(image.DOFade(0f, Duration).SetEase(Ease.InSine));
            _sequence.OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            _sequence.Kill();
            _sequence = null;
        }
    }
}