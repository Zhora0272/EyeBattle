using DG.Tweening;
using System;
using System.Collections.Generic;
using Bot.BotController;
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

    [SerializeField] private float _mapClamp = 150;

    private Dictionary<NpcBaseController, RectTransform> _mapElements;
    private List<NpcBaseController> _elements;

    public class EyeIndicator
    {
        public NpcBaseController npcBase;
        public Image _image;
    }

    private void Awake()
    {
        _mapElements = new Dictionary<NpcBaseController, RectTransform>();
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
                if (!_mapElements.ContainsKey(element))
                {
                    var indicator = Instantiate(_indicatorRectransform, _mapIndicatorContent);

                    indicator.localScale = Vector3.zero;

                    indicator.GetComponent<Image>().sprite = _indicatorSprite[0];

                    indicator.DOScale(1, 1).SetEase(Ease.OutBack);

                    _mapElements.TryAdd(element, indicator);
                }

                if (element.IsDeath.Value)
                {
                    if (_mapElements.TryGetValue(element, out var result))
                    {
                        result.GetComponent<Image>().enabled = false/*_indicatorSprite[1]*/;
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

                var anchorPosition = new Vector2(newPos.x, newPos.z) * _distance;
                
                element.Value.anchoredPosition = new Vector2
                (
                    Mathf.Clamp(anchorPosition.x, -_mapClamp, _mapClamp),
                    Mathf.Clamp(anchorPosition.y, -_mapClamp, _mapClamp)
                );
            }
        }
    }
}