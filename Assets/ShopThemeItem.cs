using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopThemeItem : MonoBehaviour
{
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] Button _button;
    [SerializeField] Image _image;

    [SerializeField] Outline _outline;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _outline = GetComponent<Outline>();
    }

    private void Start()
    {
        _button.onClick.AddListener(Selected);
    }

    private Action<int> _clickAction;
    public void SetClickCallBackAction(Action<int> _clickAction)
    {
        this._clickAction = _clickAction;
    }
    public void SetImage(Sprite image)
    {
        _image.sprite = image;
    }

    public void Selected()
    {
        _outline.DOFade(1, 0.3f);
        _clickAction.Invoke(transform.GetSiblingIndex());
    }

    public void DeselectItem()
    {
        _outline.DOFade(0, 0.3f);
    }
}
