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
    
    public void TryBuyWithMoney()
    {
        
    }

    public void TryBuyWithGem()
    {
        
    }

    public void TryBuyWithAds()
    {
        
    }
}