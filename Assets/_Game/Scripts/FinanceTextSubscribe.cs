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
        
        IReactiveProperty<int> reactiveProperty = null;
        string priceSymbol = null;
        
        switch (_buyType)
        {
            case BuyType.Money :
                reactiveProperty = _financeManager.Money;
                priceSymbol = "$";
                break;
            case BuyType.Xp :
                reactiveProperty = _financeManager.Gem;
                priceSymbol = "Xp";
                break;
        }

        reactiveProperty.Subscribe(value =>
        {
            _text.text = value + priceSymbol;
        }).AddTo(this);    
    }
}