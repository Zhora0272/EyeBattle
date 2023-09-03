using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Shop.Container
{
    public class ShopContainerPart : MonoBehaviour
    {
        [SerializeField] private float _rectHeightSize;

        [SerializeField] private CanvasGroup _activateCanvasGroup;
        [SerializeField] private CanvasGroup _deactivateCanvasGroup;

        [SerializeField] RectTransform _indicatorArrow;

        private RectTransform _rectTransform;
        private ShopContainerManager _manager;

        private float _originalRectHeightSize;
        private Button _button;

        internal bool IsActivate { get; private set; }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _rectTransform = GetComponent<RectTransform>();

            _originalRectHeightSize = _rectTransform.sizeDelta.y;
        }

        internal void SetManager(ShopContainerManager ShopContainerManager)
        {
            _manager = ShopContainerManager;

            _button.onClick.AddListener(() =>
            {
                _manager.ActivateContainer(this);
            });
        }

        internal void Activate()
        {
            IsActivate = true;

            _deactivateCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutBack).onComplete = () => { _deactivateCanvasGroup.blocksRaycasts = false; };
            _activateCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
            _activateCanvasGroup.blocksRaycasts = true;

            _rectTransform.DOSizeDelta(new Vector2(0, _rectHeightSize), 0.5f);

            _indicatorArrow.DORotate(new Vector3(0, 0, -90), 1).SetEase(Ease.OutBack);
        }

        internal void Deactivate()
        {
            IsActivate = false;

            _activateCanvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutBack).onComplete = () => { _deactivateCanvasGroup.blocksRaycasts = false; };
            _deactivateCanvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
            _deactivateCanvasGroup.blocksRaycasts = true;

            _rectTransform.DOSizeDelta(new Vector2(0, _originalRectHeightSize), 0.5f);

            _indicatorArrow.DORotate(new Vector3(0, 0, 0), 1).SetEase(Ease.OutBack);
        }
    }
}
