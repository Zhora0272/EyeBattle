using Saveing;
using System;
using UniRx;
using UnityEngine;

interface ICollectionFinanseInterface
{
    public void AddCollection(int value);
}

public class FinanceManager : MonoManager, IGameDataSaveable, ICollectionFinanseInterface
{
    //Mono Manager
    private QuestionRequestManager _questionViewManager;
    private UIManager _uiManager;

    //IReactive property
    public IReactiveProperty<int> Money => _money;
    public IReactiveProperty<int> Gem => _gem;

    //Reactive property
    private readonly ReactiveProperty<int> _money = new();
    private readonly ReactiveProperty<int> _gem = new();
    
    private readonly int _moneyCoefficient = 2;
    private readonly int _gemCoefficient = 15;

    public void AddCollection(int value)
    {
        _money.Value += value;
    }

    private void Start()
    {
        _questionViewManager = MainManager.GetManager<QuestionRequestManager>();
        _uiManager = MainManager.GetManager<UIManager>();
    }

    public int ConvertPricePointTo(BuyType type, int value)
    {
        switch (type)
        {
            case BuyType.Money: return value / _moneyCoefficient;
            case BuyType.Xp: return value / _gemCoefficient;
        }

        return default;
    }

    public int ConvertFinanceToPricePoint(BuyType type, int value)
    {
        switch (type)
        {
            case BuyType.Money: return value * _moneyCoefficient;
            case BuyType.Xp: return value * _gemCoefficient;
        }

        return default;
    }

    public void TryBuy
    (
        BuyType type,
        int pricePoint,
        Action<bool, int> responseCallBack
    )
    {
        switch (type)
        {
            case BuyType.Money:
                TryBuyWithMoney(pricePoint, responseCallBack);
                break;
            case BuyType.Xp:
                TryBuyWithGem(pricePoint, responseCallBack);
                break;
        }
    }

    private void TryBuyWithMoney
    (
        int pricePoint,
        Action<bool, int> responseCallBack
    )
    {
        TryBuyWithFinance(_money, pricePoint, responseCallBack, BuyType.Money);
    }

    private void TryBuyWithGem
    (
        int pricePoint,
        Action<bool, int> responseCallBack
    )
    {
        TryBuyWithFinance(_gem, pricePoint, responseCallBack, BuyType.Xp);
    }

    private void TryBuyWithFinance
    (
        ReactiveProperty<int> finance,
        int pricePoint,
        Action<bool, int> responseCallBack,
        BuyType type
    )
    {
        var price = ConvertPricePointTo(type, pricePoint);

        bool haveNeedFinance = finance.Value >= price;

        var headerText = $"buy with {price}";
        var confirmText = "Buy";

        if (haveNeedFinance)
        {
            _uiManager.Activate(UISubPageType.ConfirmPage);
            _questionViewManager.Activate(headerText, "Cancel", confirmText, () =>
            {
                finance.Value -= price;
                responseCallBack.Invoke(true, pricePoint);
            });
        }
        else
        {
            _uiManager.Activate(UISubPageType.ConfirmPage);
            _questionViewManager.Activate($"not enough {price}", "Cancel");
            responseCallBack.Invoke(false, pricePoint);
        }
    }
    
    public void SetData(GameData data)
    {
        _money.Value = data.Money;
        _gem.Value = data.Gem;
    }

    public GameData GetData()
    {
        return new GameData()
        {
            Money = _money.Value,
            Gem = _gem.Value
        };
    }
}