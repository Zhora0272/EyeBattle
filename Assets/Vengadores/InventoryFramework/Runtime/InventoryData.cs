using System;
using System.Collections.Generic;
using UnityEngine;
using Vengadores.DataFramework;

namespace Vengadores.InventoryFramework
{
    [Serializable]
    public class InventoryData : BaseData
    {
        public Dictionary<string, InventoryTypeModel> Models = new Dictionary<string, InventoryTypeModel>();
        
        public InventoryTypeModel GetTypeModel(string inventoryType, int initialAmount)
        {
            if (Models.TryGetValue(inventoryType, out var itemModel))
            {
                return itemModel;
            }

            itemModel = new InventoryTypeModel
            {
                Id = inventoryType,
                Amount = initialAmount
            };
            
            Models.Add(inventoryType, itemModel);
            
            return itemModel;
        }
        
        protected override void Merge(BaseData other)
        {
            var otherInventoryData = (InventoryData) other;
            
            var keys = new HashSet<string>();
            foreach (var key in Models.Keys)
            {
                keys.Add(key);
            }
            foreach (var key in otherInventoryData.Models.Keys)
            {
                keys.Add(key);
            }
            
            foreach (var key in keys)
            {
                if (otherInventoryData.Models.ContainsKey(key))
                {
                    if (Models.ContainsKey(key)) // merge
                    {
                        var otherAmount = otherInventoryData.Models[key].Amount;
                        Models[key].Amount = Mathf.Max(Models[key].Amount, otherAmount);
                    }
                    else // not in current, add it
                    {
                        Models.Add(key, otherInventoryData.Models[key].Clone());
                    }
                }
            }
        }

        public InventoryData Clone()
        {
            var clone = new InventoryData();

            foreach (var inventoryTypeModel in Models.Values)
            {
                clone.Models.Add(inventoryTypeModel.Id, inventoryTypeModel.Clone());
            }
            
            clone.OnLoaded();
            
            return clone;
        }
    }

    [Serializable]
    public class InventoryTypeModel
    {
        public string Id;
        public int Amount;

        public InventoryTypeModel Clone()
        {
            return new InventoryTypeModel
            {
                Id = Id,
                Amount = Amount
            };
        }
    }
}