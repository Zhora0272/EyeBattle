using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Vengadores.InjectionFramework;

namespace Vengadores.DataFramework.Tests.EditModeTests
{
    public class DataFrameworkEditModeTests
    {
        private DiContainer _diContainer;
        private DataManager _dataManager;

        private TestData _data;
        private TestDataOther _dataOther;

        [SetUp]
        public void SetUp()
        {
            _diContainer = new DiContainer();
            _dataManager = new DataManager();

            var handler = new TestDataHandler();
            _data = new TestData();
            _dataOther = new TestDataOther();
            
            _diContainer.RegisterInstance(_dataManager);
            _diContainer.RegisterInstance(handler);
            _diContainer.RegisterInstance(_data);
            _diContainer.RegisterInstance(_dataOther);
            
            _diContainer.Inject(_dataManager);
            _diContainer.Inject(_data);
            _diContainer.Inject(_dataOther);
        }

        [UnityTest]
        public IEnumerator Test()
        {
            yield return _dataManager.LoadAll();
            Assert.True(_dataManager.IsInitialized());
            
            Assert.True(_dataOther.IsOnLoadedCalled);
            
            _dataOther.Save(() =>
            {
                Assert.True(_dataOther.IsOnSavedCalled);
            });
            
            Assert.True(_dataOther.IsOnBeforeSaveCalled);
        }

        [TearDown]
        public void TearDown()
        {
            //
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
    
    public class TestData : BaseData
    {
        protected override void Merge(BaseData other) { }
    }
    
    public class TestDataOther : BaseData
    {
        public bool IsOnLoadedCalled;
        public bool IsOnBeforeSaveCalled;
        public bool IsOnSavedCalled;
        public override void OnLoaded()
        {
            base.OnLoaded(); // for line coverage 
            IsOnLoadedCalled = true;
        }

        public override void OnBeforeSave()
        {
            base.OnBeforeSave(); // for line coverage 
            IsOnBeforeSaveCalled = true;
        }

        public override void OnSaved()
        {
            base.OnSaved(); // for line coverage 
            IsOnSavedCalled = true;
        }
        
        protected override void Merge(BaseData other) { }
    }
}