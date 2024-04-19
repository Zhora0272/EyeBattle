public class PlayerBrokenEyeCollection : BaseBrokenEyeCollection
{
    private ICollectionFinanseInterface _financeManager;

    protected override void Start()
    {
        base.Start();
        _financeManager = MainManager.GetManager<FinanceManager>();
    }

    protected override void CollectAction(int value)
    {
        base.CollectAction(value);
        _financeManager.AddCollection(value);
    }
}