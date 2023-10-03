public class FinanceManager : MonoManager
{
    private int ConvertPricePointTo(BuyType type, int value)
    {
        int toMoneyCoefficient = 2;
        int toGemCoefficient = 15;
        int toAdsCoefficient = 50;

        switch (type)
        {
            case BuyType.Money : return value / toMoneyCoefficient;
            case BuyType.Gem : return value / toGemCoefficient;
            case BuyType.Ads : return value / toAdsCoefficient;
        }

        return 0;
    }

    public bool TryBuy()
    {
        TryBuyWithAds();
        return true;
    }
    
    private void TryBuyWithMoney()
    {
        
    }

    private void TryBuyWithGem()
    {
        
    }

    private bool TryBuyWithAds()
    {
        MainManager.GetManager<AdsManager>().TryStartAds(AdsType.RewardedAd, _ =>
        {
            print("call back");
        }, reward =>
        {
            print(reward.Amount);
            print(reward.Type);
        });

        return true;
    }
}