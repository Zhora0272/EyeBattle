using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using Vengadores.InjectionFramework;
using Vengadores.Utility.LogWrapper;

namespace Vengadores.DataFramework.Implementations
{
    public class FileSystemDataHandler : IInitializable, IDataHandler
    {
        private const string LocalDataFolderPrefix = "LocalData";
        
        public void Init()
        {
            AotHelper.EnsureList<int>();
            AotHelper.EnsureList<string>();
        }
        
        public void Load(BaseData data, Action onComplete)
        {
            var dataType = data.GetType();
            var folderPath = GetFolderPath();
            var filePath = GetFilePath(dataType.Name);

            if (!Directory.Exists(folderPath))
            {
                GameLog.LogWarning("Data",folderPath + " directory not found");
                onComplete();
                return;
            }

            if (!File.Exists(filePath))
            {
                GameLog.LogWarning("Data",filePath + " file not found");
                onComplete();
                return;
            }
            
            var json = File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(json))
            {
                GameLog.LogWarning("Data",filePath + " loaded file is empty");
                onComplete();
                return;
            }

            try
            {
                JsonConvert.PopulateObject(json, data);
                GameLog.Log("Data",filePath + " loaded from FileSystem");
            }
            catch (Exception e)
            {
                GameLog.LogError("Data",filePath + " json parse error\n" + e.Message);
            }

            onComplete();
        }

        public void Save(BaseData data, Action onComplete)
        {
            var dataType = data.GetType();
            var folderPath = GetFolderPath();
            var filePath = GetFilePath(dataType.Name);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                GameLog.LogError("Data",filePath + " json serialization error\n" + e.Message);
            }

            try
            {
                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                GameLog.LogError("Data",filePath + " error when writing the file\n" + e.Message);
            }
            
            GameLog.Log("Data", filePath + " saved to FileSystem");
            
            onComplete();
        }
        
        public static string GetFolderPath()
        {
            return Application.persistentDataPath + "/" + LocalDataFolderPrefix;
        }

        public static string GetFilePath(string dataKey)
        {
            return GetFolderPath() + "/" + dataKey + ".data";
        }
    }
}