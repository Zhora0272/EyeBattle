using System;
using UniRx;

public class FinanceManager : MonoManager, ISaveable
{
    public IReactiveProperty<int> Money => _money;
    public IReactiveProperty<int> Gem => _gem;

    private readonly ReactiveProperty<int> _money = new();
    private readonly ReactiveProperty<int> _gem = new();


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

    public void TryBuy
    (
        BuyType type,
        int price,
        Action<bool> responseCallBack
    )
    {
        switch (type)
        {
            case BuyType.Ads:
                TryBuyWithAds(responseCallBack);
                break;
            case BuyType.Money:
                TryBuyWithMoney(price, responseCallBack);
                break;
            case BuyType.Gem:
                TryBuyWithGem(price, responseCallBack);
                break;
        }
    }

    private void TryBuyWithMoney
    (
        int price,
        Action<bool> responseCallBack
    )
    {
        TryBuyWithFinance(_money, price, responseCallBack);
    }

    private void TryBuyWithGem
    (
        int price,
        Action<bool> responseCallBack
    )
    {
        TryBuyWithFinance(_gem, price, responseCallBack);
    }

    private void TryBuyWithFinance
    (
        ReactiveProperty<int> finance,
        int price,
        Action<bool> responseCallBack
    )
    {
        bool haveNeedFinance = finance.Value >= price;

        if (haveNeedFinance)
        {
            finance.Value -= price;
        }

        responseCallBack.Invoke(haveNeedFinance);
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