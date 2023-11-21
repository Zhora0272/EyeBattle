using Saveing;
using System;
using UniRx;
using UnityEngine;

public class FinanceManager : MonoManager, IGameDataSaveable
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

    private void Start()
    {
        _questionViewManager = MainManager.GetManager<QuestionRequestManager>();
        _uiManager = MainManager.GetManager<UIManager>();
    }

    public int ConvertPricePointTo(BuyType type, int value)
    {
        int toMoneyCoefficient = 2;
        int toGemCoefficient = 15;
        int toAdsCoefficient = 50;

        switch (type)
        {
            case BuyType.Money: return value / toMoneyCoefficient;
            case BuyType.Gem: return value / toGemCoefficient;
            case BuyType.Ads: return (int) ((float) value / toAdsCoefficient);
        }

        return default;
    }

    public int ConvertFinanceToPricePoint(BuyType type, int value)
    {
        int toMoneyCoefficient = 2;
        int toGemCoefficient = 15;
        int toAdsCoefficient = 50;

        switch (type)
        {
            case BuyType.Money: return value * toMoneyCoefficient;
            case BuyType.Gem: return value * toGemCoefficient;
            case BuyType.Ads: return (int) ((float) value * toAdsCoefficient);
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
            case BuyType.Ads:
                TryBuyWithAds(pricePoint, responseCallBack);
                break;
            case BuyType.Money:
                TryBuyWithMoney(pricePoint, responseCallBack);
                break;
            case BuyType.Gem:
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
        TryBuyWithFinance(_gem, pricePoint, responseCallBack, BuyType.Gem);
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
            _questionViewManager.Activate(headerText, "Cancel", confirmText, () =>
            {
                finance.Value -= price;
                responseCallBack.Invoke(true, pricePoint);
            });
        }
        else
        {
            _questionViewManager.Activate($"not enough {price}", "Cancel");
            responseCallBack.Invoke(false, pricePoint);
        }
    }

    private void TryBuyWithAds(int pricePoint, Action<bool, int> responseCallBack)
    {
        print("click to ads button");
        
        var adsCount = ConvertPricePointTo(BuyType.Ads, pricePoint);

        bool adsFinishState = false;

        string headerText;
        string confirmText;

        headerText = "Watch Ads to Unlock";
        confirmText = "Watch";

        Action _action = null;

        _uiManager.Activate(UISubPageType.ConfirmPage);
        _questionViewManager.Activate(headerText, "Cancel", confirmText, () =>
        {
            MainManager.GetManager<AdsManager>().TryStartAds(AdsType.RewardedAd,
                showState =>
                {
                    if (showState)
                    {
                        headerText = $"Watch Ads to Unlock";
                        confirmText = "Watch";

                        _questionViewManager.Activate(headerText, "Cancel", confirmText, () =>
                        {
                            _action?.Invoke();
                        });
                    }
                    else
                    {
                        if (Application.internetReachability == NetworkReachability.NotReachable)
                        {
                            headerText = "Internet connection error!";
                        }
                        else
                        {
                            headerText = "Oops ads unavailable";
                        }

                        _questionViewManager.Activate(headerText, "Ok");
                    }
                }, out var adsStartAction,
                reward =>
                {
                    adsCount--;

                    var price = ConvertFinanceToPricePoint(BuyType.Ads, adsCount);

                    adsFinishState = adsCount == 0;
                    responseCallBack.Invoke(adsFinishState, price);
                });
            {
                _action = adsStartAction;
            }
        });

    }

    private void AdsRewardAction()
    {
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