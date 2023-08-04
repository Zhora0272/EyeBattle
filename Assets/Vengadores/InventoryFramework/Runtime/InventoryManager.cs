using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vengadores.InjectionFramework;
using Vengadores.SignalHub;
using IDisposable = Vengadores.InjectionFramework.IDisposable;

namespace Vengadores.InventoryFramework
{
    public class InventoryManager : IDisposable
    {
        [Inject] private SignalHub.SignalHub _signalHub;
        [Inject] private IInventoryHandler _handler;
        [Inject] private InventoryData _inventoryData;
        
        private InventoryData _inventoryDataPushed;

        private List<InventoryCommit> _commits = new List<InventoryCommit>();

        private Dictionary<string, Action<InventoryChangeInfo>> _onChangeEvent = new Dictionary<string, Action<InventoryChangeInfo>>();

        [PublicAPI] public void Initialize()
        {
            _inventoryDataPushed = _inventoryData.Clone();
            
            _inventoryData.OnMergedCallback += OnMerged;
        }
        
        public void Dispose()
        {
            _inventoryData.OnMergedCallback -= OnMerged;
        }

        [PublicAPI] public int Get(string type)
        {
            var initialAmount = _handler.GetInitialAmount(type);
            var typeModel = _inventoryDataPushed.GetTypeModel(type, initialAmount);
            return typeModel.Amount;
        }

        [PublicAPI] public InventoryCommit Commit(string type, int amountToAdd)
        {
            var initialAmount = _handler.GetInitialAmount(type);
            var committedTypeModel = _inventoryData.GetTypeModel(type, initialAmount);

            var currentAmount = committedTypeModel.Amount;
            var clampedNewAmount = _handler.Clamp(type, currentAmount + amountToAdd);
            var diff = clampedNewAmount - currentAmount;

            if (diff == 0) return null;

            committedTypeModel.Amount = clampedNewAmount;
            _inventoryData.Save();

            var commit =  new InventoryCommit()
            {
                InventoryType = type,
                AmountToAdd = diff
            };
            
            _commits.Add(commit);

            return commit;
        }

        [PublicAPI] public void Push(InventoryCommit commit)
        {
            if(commit == null) return;
            
            if(!_commits.Contains(commit)) return;

            _commits.Remove(commit);

            var initialAmount = _handler.GetInitialAmount(commit.InventoryType);
            var pushedTypeModel = _inventoryDataPushed.GetTypeModel(commit.InventoryType, initialAmount);

            var previousAmount = pushedTypeModel.Amount;
            pushedTypeModel.Amount += commit.AmountToAdd;

            var inventoryChangeInfo = new InventoryChangeInfo()
            {
                Type = commit.InventoryType,
                PreviousAmount = previousAmount,
                CurrentAmount = pushedTypeModel.Amount
            };
            _signalHub.Get<InventoryChangedSignal>().Dispatch(inventoryChangeInfo);
            
            if (_onChangeEvent.ContainsKey(commit.InventoryType))
            {
                _onChangeEvent[commit.InventoryType]?.Invoke(inventoryChangeInfo);
            }
        }

        [PublicAPI] public void AddAndSync(string type, int amountToAdd)
        {
            Push(Commit(type, amountToAdd));
        }
        
        [PublicAPI] public void SetAndSync(string type, int amountToSet)
        {
            Push(Commit(type, amountToSet - Get(type)));
        }
            
        [PublicAPI] public void AddOnChangeListener(string type, Action<InventoryChangeInfo> cb)
        {
            if (!_onChangeEvent.ContainsKey(type))
            {
                _onChangeEvent[type] = cb;
            }
            else
            {
                _onChangeEvent[type] += cb;
            }
        }
        
        [PublicAPI] public void RemoveOnChangeListener(string type, Action<InventoryChangeInfo> cb)
        {
            _onChangeEvent[type] -= cb;
        }
        
        private void OnMerged()
        {
            // Sync pushed data
            _commits.Clear();
            _inventoryDataPushed = _inventoryData.Clone();
        }
    }

    public struct InventoryChangeInfo
    {
        public string Type;
        public int PreviousAmount;
        public int CurrentAmount;
    }
    
    public class InventoryChangedSignal : ASignal<InventoryChangeInfo> {}
}
