using System;
using NUnit.Framework;
using Vengadores.DataFramework;
using Vengadores.InjectionFramework;

namespace Vengadores.InventoryFramework.Tests.EditModeTests
{
    public class InventoryTests
    {
        InventoryManager SetupInventoryWithDI()
        {
            var diContainer = new DiContainer();
            var signalHub = new SignalHub.SignalHub();
            var inventoryHandler = new TestInventoryHandler();
            var inventoryData = new InventoryData();
            var dataManager = new DataManager();
            var dataHandler = new TestDataHandler();
            var inventory = new InventoryManager();
            
            diContainer.RegisterInstance(signalHub);
            diContainer.RegisterInstance(inventoryHandler);
            diContainer.RegisterInstance(inventoryData);
            diContainer.RegisterInstance(dataManager);
            diContainer.RegisterInstance(dataHandler);
            
            diContainer.Inject(dataManager);
            diContainer.Inject(inventoryData);
            diContainer.Inject(inventory);
            
            inventory.Initialize();
            return inventory;
        }
        
        [Test]
        public void InventoryDataTest()
        {
            var data = new InventoryData();

            // From new
            var testData = data.GetTypeModel("DummyType", 0);
            Assert.NotNull(testData);
            
            // From cache
            var testData2 = data.GetTypeModel("DummyType", 0);
            Assert.NotNull(testData2);
            Assert.AreEqual(testData, testData2);

            var cloned = data.Clone();
            var testData3 = cloned.GetTypeModel("DummyType", 0);
            Assert.NotNull(testData3);
            Assert.AreEqual(testData.Id, testData3.Id);
            Assert.AreEqual(testData.Amount, testData3.Amount);
        }

        [Test]
        public void InventoryTest()
        {
            var inventory = SetupInventoryWithDI();
            
            Assert.AreEqual(inventory.Get("DummyType"), 0);
            
            var commit = inventory.Commit("DummyType", 5);
            Assert.NotNull(commit);
            Assert.AreEqual(inventory.Get("DummyType"), 0);
            
            inventory.Push(commit);
            Assert.AreEqual(inventory.Get("DummyType"), 5);
            
            // same commit again
            inventory.Push(commit);
            Assert.AreEqual(inventory.Get("DummyType"), 5);
            
            inventory.Push(null);
            Assert.AreEqual(inventory.Get("DummyType"), 5);
            
            inventory.AddAndSync("DummyType", 10);
            Assert.AreEqual(inventory.Get("DummyType"), 15);
        }

        
        // Test the callback ways. 
        private int _gold1 = 0;
        private int _gold2 = 0;
        private int _bars1 = 0;
        public const string Coin = "Coin";
        public const string GoldBar = "G-Barz";

        [Test]
        public void OnChangeTest()
        {
            var inventory = SetupInventoryWithDI();
            _gold1 = 0;
            _gold2 = 0;
            _bars1 = 0;
            
            // simple use.
            inventory.AddOnChangeListener(Coin,OnGoldChanged);
            inventory.SetAndSync(Coin,100);
            inventory.AddAndSync(Coin,25);
            inventory.AddAndSync(GoldBar,25);
            Assert.AreEqual(2,_gold1);
            
            // add a double listener
            inventory.AddOnChangeListener(Coin,OnGoldChanged2);
            inventory.AddAndSync(Coin,25);
            Assert.AreEqual(3,_gold1);
            Assert.AreEqual(1,_gold2);
            Assert.AreEqual(0,_bars1);
            
            // add a bars listener
            inventory.AddOnChangeListener(GoldBar,OnBarsChanged);
            inventory.AddAndSync(Coin,24);
            inventory.AddAndSync(GoldBar,24);
            Assert.AreEqual(4,_gold1);
            Assert.AreEqual(2,_gold2);
            Assert.AreEqual(1,_bars1);

            // remove a coin listener
            inventory.RemoveOnChangeListener(Coin,OnGoldChanged);
            inventory.AddAndSync(Coin,11);
            inventory.AddAndSync(GoldBar,11);
            Assert.AreEqual(4,_gold1);
            Assert.AreEqual(3,_gold2);
            Assert.AreEqual(2,_bars1);
            
            // remove other coin listener
            inventory.RemoveOnChangeListener(Coin,OnGoldChanged2);
            inventory.AddAndSync(Coin,7);
            inventory.AddAndSync(GoldBar,7);
            Assert.AreEqual(4,_gold1);
            Assert.AreEqual(3,_gold2);
            Assert.AreEqual(3,_bars1);

        }

        private void OnGoldChanged(InventoryChangeInfo info)
        {
            _gold1++;
        }
        private void OnGoldChanged2(InventoryChangeInfo info)
        {
            _gold2++;
        }
        private void OnBarsChanged(InventoryChangeInfo info)
        {
            _bars1++;
        }
    }

    public class TestInventoryHandler : IInventoryHandler
    {
        public int GetInitialAmount(string type)
        {
            return 0;
        }

        public int Clamp(string type, int amount)
        {
            return amount;
        }
    }
    
    public class TestDataHandler : IDataHandler
    {
        public void Load(BaseData data, Action onComplete)
        {
            onComplete();
        }

        public void Save(BaseData data, Action onComplete)
        {
            onComplete();
        }
    }
}