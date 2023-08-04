using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using Vengadores.InjectionFramework;
using Vengadores.Utility.LogWrapper;

namespace Vengadores.DataFramework.Implementations
{
    public class PlayerPrefsDataHandler : IInitializable, IDataHandler
    {
        private const string Prefix = "";
        
        public void Init()
        {
            AotHelper.EnsureList<int>();
            AotHelper.EnsureList<string>();
        }
        
        public void Load(BaseData data, Action onComplete)
        {
            var rawData = PlayerPrefs.GetString(GetPath(data.GetType()));
            
            if (!string.IsNullOrEmpty(rawData))
            {
                try
                {
                    JsonConvert.PopulateObject(rawData, data);
                    GameLog.Log("Data",GetPath(data.GetType()) + " loaded from PlayerPrefs");
                }
                catch (Exception e)
                {
                    GameLog.LogError("Data",GetPath(data.GetType()) + " json parse error\n" + e.Message);
                }
            }
            
            onComplete();
        }

        public void Save(BaseData data, Action onComplete)
        {
            var dataType = data.GetType();
            
            var json = string.Empty;
            try
            {
                json = JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                GameLog.LogError("Data",GetPath(dataType) + " json serialization error\n" + e.Message);
            }

            PlayerPrefs.SetString(GetPath(dataType), json);
            PlayerPrefs.Save();
            
            GameLog.Log("Data", GetPath(dataType) + " saved to PlayerPrefs");
            
            onComplete();
        }

        private string GetPath(Type dataType)
        {
            return Prefix + dataType.Name;
        }
    }
}
