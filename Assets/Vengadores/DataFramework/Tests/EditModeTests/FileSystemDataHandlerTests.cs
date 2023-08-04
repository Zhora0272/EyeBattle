using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Vengadores.DataFramework.Implementations;

namespace Vengadores.DataFramework.Tests.EditModeTests
{
    public class FileSystemDataHandlerTests
    {
        [SetUp]
        public void SetUp()
        {
            var filepath = FileSystemDataHandler.GetFilePath(nameof(TestClass));
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            
            var folder = FileSystemDataHandler.GetFolderPath();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        
        [UnityTest]
        public IEnumerator SaveLoadTests()
        {
            var filepath = FileSystemDataHandler.GetFilePath(nameof(TestClass));
            var handler = new FileSystemDataHandler();
            var model = new TestClass();

            handler.Load(model, () =>
            {
                Assert.NotNull(model);
                Assert.Null(model.testDict);
            });
            LogAssert.Expect(LogType.Warning, new Regex(""));

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
            
            File.WriteAllText(filepath, "CorruptedJsonData...");

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
            var filepath = FileSystemDataHandler.GetFilePath(nameof(TestClass));
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }
    }
}