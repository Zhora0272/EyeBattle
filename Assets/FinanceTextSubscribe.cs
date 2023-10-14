using System;
using TMPro;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FinanceTextSubscribe : MonoBehaviour
{
    [SerializeField] private BuyType _buyType;

    private FinanceManager _financeManager;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _financeManager = MainManager.GetManager<FinanceManager>();
        
        switch (_buyType)
        {
            case BuyType.Money :
                _financeManager.Money.Subscribe(value =>
                {
                    _text.text = value + "$";
                }).AddTo(this);
                break;
            case BuyType.Gem :
                _financeManager.Gem.Subscribe(value =>
                {
                    _text.text = value + "#";
                }).AddTo(this);
                break;
        }
    }
}