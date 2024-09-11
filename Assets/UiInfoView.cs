using System;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UiInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private Slider _slider;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Activate
    (
        string header,
        string description,
        float sliderValue,
        float sliderMin,
        float sliderMax
    )
    {
        _header.text = header;
        _description.text = description;

        _slider.minValue = sliderMin;
        _slider.maxValue = sliderMax;

        _slider.value = sliderValue;

        _canvasGroup.DOFade(1, 1);
    }

    public void ActivateTimed
    (
        float messageTime,
        string header,
        string description,
        float sliderValue,
        float sliderMin,
        float sliderMax
    )
    {
        Activate(header, description, sliderValue, sliderMin, sliderMax);
        
        Observable.Timer(TimeSpan.FromSeconds(messageTime)).Subscribe(_ =>
        {
            Deactivate();

        }).AddTo(this);
    }

    public void Deactivate()
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0, 1);
    }
}