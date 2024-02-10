using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private Sprite[] _indicatorSprite;
    [SerializeField] private RectTransform _indicatorRectransform;

    [SerializeField] private RectTransform _playerIndicator;

    [SerializeField] private Transform _playerTransform;

    [SerializeField] private Transform _mapIndicatorContent;

    [SerializeField] private float _distance;

    private Dictionary<EyeBaseController, RectTransform> _mapElements;
    private List<EyeBaseController> _elements;

    public class EyeIndicator
    {
        public EyeBaseController _eyeBase;
        public Image _image;
    }

    private void Awake()
    {
        _mapElements = new Dictionary<EyeBaseController, RectTransform>();
    }

    //bad code need optimization
    private void Start()
    {
        _elements = MainManager.GetManager<EyeSpawnManager>()._spawnedEyes;

        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => 
        {
            var count = _mapElements.Count;

            foreach (var element in _elements)
            {
                if(!_mapElements.ContainsKey(element))
                {
                    var indicator = Instantiate(_indicatorRectransform, _mapIndicatorContent);

                    indicator.localScale = Vector3.zero;

                    indicator.GetComponent<Image>().sprite = _indicatorSprite[0];

                    indicator.DOScale(1, 1).SetEase(Ease.OutBack);

                    _mapElements.TryAdd(element, indicator);
                }

                if(element.IsDeath.Value)
                {
                    if(_mapElements.TryGetValue(element, out var result))
                    {
                        result.GetComponent<Image>().sprite = _indicatorSprite[1];
                    }
                }
            }

        }).AddTo(this);
    }

    private void Update()
    {
        foreach (var element in _mapElements)
        {
            if (element.Value)
            {
                var item = element.Key.transform;

                var newPos = item.position - _playerTransform.position;

                element.Value.anchoredPosition = new Vector2(newPos.x, newPos.z) * _distance;

                /*if (element.Key.Size.Value * 15 != element.Value.sizeDelta.x)
                {
                    var newSizeDelta = new Vector2(element.Key.Size.Value, element.Key.Size.Value);

                    element.Value.DOSizeDelta(newSizeDelta * 15, 1);
                }*/
            }
        }
    }
}
