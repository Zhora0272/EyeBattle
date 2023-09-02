using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPage : MonoBehaviour
{
    [field: SerializeField] public UIPageType PageTye { get; private set; }

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _canvasGroup.DOFade(1, 0.5f);
    }

    public void Deactivate()
    {
        _canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
