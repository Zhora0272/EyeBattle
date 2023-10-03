using Shop;

public interface IEyeItemParameters
{
    public int Index { get; set; }
    public ShopItemState ItemState { get; set; }
    public BuyType BuyType { get; set; }
    public int PricePoint { get; set; } //price point is a price value for local generic calculation for moneys and ads 
}