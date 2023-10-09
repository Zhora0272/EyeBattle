public interface IDataSave
{
    public string Path { get; set; }
    public void SaveData(GameData data);
    public GameData GetData();
    public bool ExistData();
}