using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vengadores.DataFramework.Implementations;

namespace Vengadores.DataFramework.Tests.EditModeTests
{
    public class PlayerPrefsDataHandlerTests
    {

        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey(nameof(TestClass));
        }
        
        [UnityTest]
        public IEnumerator SaveLoadTests()
        {
            var handler = new PlayerPrefsDataHandler();
            var model = new TestClass();

            handler.Load(model, () =>
            {
                Assert.NotNull(model);
                Assert.Null(model.testDict);
            });

            yield return null;
            
            model.testDict = new Dictionary<string, int>();
            model.testDict.Add("A", 1);
            model.testDict.Add("B", 2);
            model.testString = "Test";
            model.testArray = new[] {true, false, true};
            
            handler.Save(model, () => { });
            
            yield return null;
            
            handler.Load(model, () =>
            {
                Assert.NotNull(model);
                Assert.AreEqual(model.testDict.Count, 2);
                Assert.AreEqual(model.testString, "Test");
                Assert.AreEqual(model.testArray.Length, 3);
            });
            
            yield return null;

            model.testString = "ChangedTestVal";
            
            PlayerPrefs.SetString(model.GetType().Name, "CorruptedJsonData...");
            PlayerPrefs.Save();
            
            handler.Load(model, () =>
            {
                Assert.NotNull(model);
                Assert.AreNotEqual(model.testString, "Test");
            });
            
            LogAssert.Expect(LogType.Error, new Regex(""));
            
            yield return null;
        }
        
        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey(nameof(TestClass));
        }
    }

    [Serializable]
    public class TestClass : BaseData
    {
        public Dictionary<string, int> testDict;

        public string testString;

        public bool[] testArray;
        
        protected override void Merge(BaseData other) { }
    }
}