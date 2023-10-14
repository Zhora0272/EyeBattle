using System.IO;
using UnityEngine;

public class JsonHelper : IDataSave
{
    public string Path { get; set; }

    public JsonHelper()
    {
        Path = Application.persistentDataPath + "/" + "JsonData.json";
    }

    public void SaveData(GameData data)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        
        //var hashData = HashManager.HashData(jsonData);
        
        using (StreamWriter writer = new StreamWriter(Path))
        {
            writer.Write(jsonData);
        }
    }

    public GameData GetData()
    {
        using (StreamReader reader = new StreamReader(Path))
        {
            string jsonData = reader.ReadToEnd();

            //var hashData = HashManager.DeHashData(jsonData);

            return JsonUtility.FromJson<GameData>(jsonData);
        }
    }

    public bool ExistData()
    {
        return File.Exists(Path);
    }
}