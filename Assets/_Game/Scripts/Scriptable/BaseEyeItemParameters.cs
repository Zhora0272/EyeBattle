using System;
using Shop;

[Serializable]
public class BaseEyeItemParameters
{
    public int Index;
    public ShopItemState ItemState;
    public BuyType BuyType;
    public int PricePoint;
}