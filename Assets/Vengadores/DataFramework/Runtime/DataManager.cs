using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Vengadores.InjectionFramework;

namespace Vengadores.DataFramework
{
    public class DataManager
    {
        [Inject] private IDataHandler _dataHandler; 
        [Inject] private IData[] _dataArray; 
        
        private readonly Dictionary<Type, BaseData> _dataByTypes = new Dictionary<Type, BaseData>();

        private bool _isInitialized;
        
        [PublicAPI] public IEnumerator LoadAll()
        {
            _dataByTypes.Clear();
            
            var loadedCount = 0;
            foreach (var data in _dataArray)
            {
                var baseData = (BaseData) data;
                _dataHandler.Load(baseData, () =>
                {
                    _dataByTypes.Add(baseData.GetType(), baseData);
                    loadedCount++;
                    baseData.OnLoaded();
                });
            }

            yield return new WaitUntil(() => loadedCount == _dataArray.Length);

            _isInitialized = true;
        }

        [PublicAPI] public bool IsInitialized()
        {
            return _isInitialized;
        }

        [PublicAPI] public BaseData GetBaseData(Type baseDataType)
        {
            return _dataByTypes.TryGetValue(baseDataType, out var result) ? result : null;
        }

        internal void Save(BaseData data, Action onComplete)
        {
            _dataHandler.Save(data, onComplete);
        }
    }
}