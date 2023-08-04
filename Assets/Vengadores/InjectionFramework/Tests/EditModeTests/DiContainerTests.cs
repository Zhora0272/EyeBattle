using System;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Vengadores.InjectionFramework.Tests.EditModeTests
{
    public class DiContainerTests
    {
        private DiContainer _diContainer;

        [SetUp]
        public void SetUp()
        {
            _diContainer = new DiContainer();
        }
        
        [Test]
        public void DiContainerGet()
        {
            Assert.Throws<NullReferenceException>(() => _diContainer.Get<TestClass>());
            Assert.Throws<NullReferenceException>(() => _diContainer.Get<TestClass>("DummyId"));
            Assert.Throws<NullReferenceException>(() => _diContainer.GetAll<TestClass>());

            // One instance without id
            var instance = new TestClass();
            _diContainer.RegisterInstance(instance);
            Assert.DoesNotThrow(() => _diContainer.Get<TestClass>());
            Assert.Throws<NullReferenceException>(() => _diContainer.Get<TestClass>("DummyId"));
            Assert.DoesNotThrow(() => _diContainer.GetAll<TestClass>());
            Assert.AreEqual(_diContainer.GetAll<TestClass>().Length, 1);
            _diContainer.UnregisterInstance(instance);
            
            // One instance with id
            var instanceA = new TestClass();
            _diContainer.RegisterInstance(instanceA, "WithId");
            Assert.DoesNotThrow(() => _diContainer.Get<TestClass>("WithId"));
            Assert.Throws<NullReferenceException>(() => _diContainer.Get<TestClass>("DummyId"));
            Assert.DoesNotThrow(() => _diContainer.GetAll<TestClass>());
            Assert.AreEqual(_diContainer.GetAll<TestClass>().Length, 1);
            
            // Multiple instances
            var instanceB = new TestClass();
            var instanceC = new TestClass();
            var instanceD = new TestClass();
            _diContainer.RegisterInstance(instanceB);
            _diContainer.RegisterInstance(instanceC);
            _diContainer.RegisterInstance(instanceD, "AnotherId");
            Assert.DoesNotThrow(() => _diContainer.Get<TestClass>("WithId"));
            Assert.DoesNotThrow(() => _diContainer.Get<TestClass>());
            Assert.DoesNotThrow(() => _diContainer.GetAll<TestClass>());
            Assert.AreEqual(_diContainer.GetAll<TestClass>().Length, 4);

            // Removal
            _diContainer.UnregisterInstance(instanceA, "WithId");
            _diContainer.UnregisterInstance(instanceB);
            _diContainer.UnregisterInstance(instanceC);
            _diContainer.UnregisterInstance(instanceD, "AnotherId");
            Assert.Throws<NullReferenceException>(() => _diContainer.GetAll<TestClass>());
            
            // Remove again
            _diContainer.UnregisterInstance(instanceA);
            _diContainer.UnregisterInstance(instanceD, "AnotherId");
            
            // Multiple interfaces and interface access
            var a = new TestClassWithInterface();
            var b = new TestClassWithInterface();
            _diContainer.RegisterInstance(a);
            _diContainer.RegisterInstance(b, "Id");
            Assert.DoesNotThrow(() => _diContainer.GetAll<ITestInterface>());
            Assert.AreEqual(_diContainer.GetAll<ITestInterface>().Length, 2);
            
            // Removal
            _diContainer.UnregisterInstance(a);
            _diContainer.UnregisterInstance(b, "Id");
        }

        [Test]
        public void DiContainerInject()
        {
            var testInstance = new TestClass();
            var testInstanceWithInjectable = new TestClassWithInjectable();
            var testInstanceWithInjectableArray = new TestClassWithInjectableArray();
            var testInstanceWithIdError = new TestClassWithIdError();
            var testInstanceWithTypeError = new TestClassWithTypeError();
            var testInstanceWithArrayTypeError = new TestClassWithArrayTypeError();
            
            // not injectable, not in cache
            _diContainer.Inject(testInstance);
            // not injectable but cached
            _diContainer.Inject(testInstance);
            
            // dummy instances
            _diContainer.RegisterInstance("InjectedString");
            _diContainer.RegisterInstance("InjectedAnotherString");
            _diContainer.RegisterInstance(5);

            // injectable
            _diContainer.Inject(testInstanceWithInjectable);
            // inject again to get from reflection cache
            _diContainer.Inject(testInstanceWithInjectable);
            // First instance will be given
            Assert.NotNull(testInstanceWithInjectable.GetTestString());
            Assert.AreEqual(testInstanceWithInjectable.GetTestString(), "InjectedString");

            // injectable array
            _diContainer.Inject(testInstanceWithInjectableArray);
            Assert.NotNull(testInstanceWithInjectableArray.GetTestStringArray());
            Assert.AreEqual(testInstanceWithInjectableArray.GetTestStringArray().Length, 2);
            Assert.AreEqual(testInstanceWithInjectableArray.GetTestInt(), 5);
            
            // id error
            Assert.Throws<NullReferenceException>(() => _diContainer.Inject(testInstanceWithIdError));
            LogAssert.Expect(LogType.Error, new Regex(""));
            
            // type error
            Assert.Throws<NullReferenceException>(() => _diContainer.Inject(testInstanceWithTypeError));
            LogAssert.Expect(LogType.Error, new Regex(""));
            
            // array type error
            Assert.Throws<TargetInvocationException>(() => _diContainer.Inject(testInstanceWithArrayTypeError));
            LogAssert.Expect(LogType.Error, new Regex(""));
        }
    }

    public interface ITestInterface { }
    
    public class TestClass {}
    
    public class TestClassWithInterface : ITestInterface {}

    public class TestClassWithInjectable
    {
        [Inject] private string TestString;

        public string GetTestString()
        {
            return TestString;
        }
    }
    
    public class TestClassWithInjectableArray
    {
        [Inject] private int TestInt;
        [Inject] private string[] TestStringArray;
        
        public int GetTestInt()
        {
            return TestInt;
        }
        
        public string[] GetTestStringArray()
        {
            return TestStringArray;
        }
    }
    
    public class TestClassWithIdError
    {
        [Inject("NotExists")] private int TestInt;
        
        public int GetTestInt()
        {
            return TestInt;
        }
    }
    
    public class TestClassWithTypeError
    {
        [Inject] private char TestChar;
        
        public char GetTestChar()
        {
            return TestChar;
        }
    }
    
    public class TestClassWithArrayTypeError
    {
        [Inject] private float[] TestFloatArray;
        
        public float[] GetTestFloatArray()
        {
            return TestFloatArray;
        }
    }
}