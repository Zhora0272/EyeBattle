using DG.Tweening;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _notificationAlpha;
    [SerializeField] private CanvasGroup _notificationTextAlpha;
    [SerializeField] private RectTransform _rectTransform;

    private Vector2 _sizeDelta;

    private void Activate()
    {
        _rectTransform.DOSizeDelta(_sizeDelta, 0.5f);
        _notificationTextAlpha.DOFade(1, 0.5f).SetEase(Ease.OutBack);
    }

    private void Deactivate()
    {
        _rectTransform.DOSizeDelta(Vector2.zero, 0.5f);
        _notificationTextAlpha.DOFade(0, 0.5f).SetEase(Ease.OutBack);
    }

}
