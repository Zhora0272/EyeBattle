
namespace Saveing
{
    public interface IGameDataSaveable : ISaveable<GameData> { }
    
    public interface IEyeItemCollectionSaveable : ISaveable<EyeItemCollection> { }
    public interface IEyeBaseItemParametersSaveable : ISaveable<BaseEyeItemParameters> { }
    
    public interface IShopContainerSaveable : ISaveable<(int,EyeItemCollection)>{ }

    public interface ISaveable<T>
    {
        public void SetData(T data);
        public T GetData();
    }

}
