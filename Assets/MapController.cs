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
        _elements = MainManager.GetManager<EyeSpawnManager>()._spawnEyes;

        Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ => 
        {
            print(_elements.Count);

            var count = _mapElements.Count;

            foreach (var element in _elements)
            {
                if(!_mapElements.ContainsKey(element))
                {
                    var indicator = Instantiate(_indicatorRectransform, _mapIndicatorContent);

                    indicator.GetComponent<Image>().sprite = _indicatorSprite[0];

                    _mapElements.TryAdd(element, indicator);
                }

                if(element.IsDeath)
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
            }
        }
    }
}
