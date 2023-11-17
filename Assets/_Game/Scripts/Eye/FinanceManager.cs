using System;
using Saveing;
using UniRx;

public class FinanceManager : MonoManager, IGameDataSaveable
{
    
    private QuestionRequestManager _questionViewManager;

    public IReactiveProperty<int> Money => _money;
    public IReactiveProperty<int> Gem => _gem;

    private readonly ReactiveProperty<int> _money = new();
    private readonly ReactiveProperty<int> _gem = new();

    private void Start()
    {
        _questionViewManager = MainManager.GetManager<QuestionRequestManager>();
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
            case BuyType.Ads:
            {
                float adsCoefficient = (float) value / toAdsCoefficient;

                if (adsCoefficient > 0.5f) adsCoefficient = 1;

                return (int) adsCoefficient;
            }
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
            case BuyType.Money: return value / toMoneyCoefficient;
            case BuyType.Gem: return value / toGemCoefficient;
            case BuyType.Ads:
            {
                float adsCoefficient = (float) value / toAdsCoefficient;

                if (adsCoefficient > 0.5f) adsCoefficient = 1;

                return (int) adsCoefficient;
            }
        }

        return default;
    }

    public void TryBuy
    (
        BuyType type,
        int pricePoint,
        Action<bool> responseCallBack
    )
    {
        switch (type)
        {
            case BuyType.Ads:
                TryBuyWithAds(responseCallBack);
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
        Action<bool> responseCallBack
    )
    {
        TryBuyWithFinance(_money, pricePoint, responseCallBack, BuyType.Money);
    }

    private void TryBuyWithGem
    (
        int pricePoint,
        Action<bool> responseCallBack
    )
    {
        TryBuyWithFinance(_gem, pricePoint, responseCallBack, BuyType.Gem);
    }

    private void TryBuyWithFinance
    (
        ReactiveProperty<int> finance,
        int pricePoint,
        Action<bool> responseCallBack, BuyType type
    )
    {
        var price = ConvertPricePointTo(type, pricePoint);
        
        bool haveNeedFinance = finance.Value >= price;

        if (haveNeedFinance)
        {
            _questionViewManager.Activate($"buy with {price}", "Cancel", "Buy", () =>
            {
                finance.Value -= price;
                responseCallBack.Invoke(true);
            });
        }
        else
        {
            _questionViewManager.Activate($"not enough {price}", "Cancel");
            responseCallBack.Invoke(false);
        }
    }

    private void TryBuyWithAds(Action<bool> responseCallBack)
    {
        bool adsFinishState = false;

        MainManager.GetManager<AdsManager>().TryStartAds(AdsType.RewardedAd,
            reward =>
            {
                adsFinishState = true;

                responseCallBack.Invoke(adsFinishState);
            });
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