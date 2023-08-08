using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopThemeItem : MonoBehaviour
{
    [SerializeField] RectTransform _rectTransform;
    [SerializeField] Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetImage(Sprite image)
    {
        _image.sprite = image;
    }
}
