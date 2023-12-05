using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Container
{
    public class ShopContainer : MonoBehaviour
    {
        [SerializeField] private float _rectHeightSize;

        [SerializeField] private CanvasGroup _activateCanvasGroup;
        [SerializeField] private CanvasGroup _deactivateCanvasGroup;

        [SerializeField] private RectTransform _indicatorArrow;

        public IReactiveProperty<bool> IsActivated => _isActivated;
        private readonly ReactiveProperty<bool> _isActivated = new();

        private RectTransform _rectTransform;
        private ShopContainerManager _manager;

        private float _originalRectHeightSize;
        private Button _button;
        

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rectTransform = GetComponent<RectTransform>();
            _originalRectHeightSize = _rectTransform.sizeDelta.y;
        }

        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                if (_manager)
                {
                    _manager.ActivateContainer(this);
                }
            });
            IsActivated.Subscribe(state =>
            {
                _button.enabled = !state;
            }).AddTo(this);
        }

        internal void SetManager(ShopContainerManager ShopContainerManager)
        {
            _manager = ShopContainerManager;
        }

        internal void Activate()
        {
            _isActivated.Value = true;

            _deactivateCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
            {
                _deactivateCanvasGroup.blocksRaycasts = false;
            };
            _activateCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
            _activateCanvasGroup.blocksRaycasts = true;

            _rectTransform.DOSizeDelta(new Vector2(0, _rectHeightSize), 0.5f);

            _indicatorArrow.DORotate(new Vector3(0, 0, -90), 1).SetEase(Ease.OutBack);
        }
        internal void Deactivate()
        {
            _isActivated.Value = false;

            _activateCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
            {
                _deactivateCanvasGroup.blocksRaycasts = false;
            };
            _deactivateCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
            _deactivateCanvasGroup.blocksRaycasts = true;

            _rectTransform.DOSizeDelta(new Vector2(0, _originalRectHeightSize), 0.5f);

            _indicatorArrow.DORotate(new Vector3(0, 0, 0), 1).SetEase(Ease.OutBack);
        }
    }
}