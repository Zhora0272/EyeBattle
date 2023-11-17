using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPageBase : MonoBehaviour, IOpenCloseable
    {
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
    
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }
    
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, 0.5f);
        }

        public virtual void Deactivate()
        {
            _canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}