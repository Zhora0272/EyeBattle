using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _closeButton.onClick.AddListener(() =>
        {
            Deactivate();
        });
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _rectTransform.DOAnchorPos(new Vector2(0,0), 1);
    }

    public void Deactivate()
    {
        _rectTransform.DOAnchorPos(new Vector2(_rectTransform.sizeDelta.x,0), 1).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
