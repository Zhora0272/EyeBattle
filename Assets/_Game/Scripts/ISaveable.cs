public interface ISaveable : IBaseSaveable<GameData>
{
  
}

public interface IEyeItemSaveable : IBaseSaveable<EyeItemCollection>
{
    
}

public interface IBaseSaveable<T>
{
    public void SetData(T data);
    public T GetData();
}
