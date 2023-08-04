namespace Vengadores.InventoryFramework
{
    public interface IInventoryHandler
    {
        int GetInitialAmount(string type);

        int Clamp(string type, int amount);
    }
}