using System;
using System.Data;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vengadores.SignalHub.Tests.EditModeTests
{
    public class SignalHubEditModeTests
    {
        private SignalHub _signalHub;
        private GameObject _gameObjectToAttach;
        
        [SetUp]
        public void SetUp()
        {
            _signalHub = new SignalHub();
            _gameObjectToAttach = new GameObject();
        }
        
        [Test]
        public void TestGet()
        {
            // First get will create a signal instance and cache
            var signalWithNoParameter = _signalHub.Get<TestSignalNoParameter>();
            Assert.NotNull(signalWithNoParameter);
            
            // Get from the cache
            var signalWithNoParameterFromCache = _signalHub.Get<TestSignalNoParameter>();
            Assert.NotNull(signalWithNoParameterFromCache);
        }

        [Test]
        public void TestSignalWithNoParameter()
        {
            var signalWithNoParameter = _signalHub.Get<TestSignalNoParameter>();
            Assert.NotNull(signalWithNoParameter);

            var callbackReceived = false;
            var otherCallbackReceived = false;
            var callback = new Action(() => { callbackReceived = true;});
            var otherCallback = new Action(() => { otherCallbackReceived = true;});
            
            // No listeners to remove
            signalWithNoParameter.RemoveListener(_gameObjectToAttach);
            // No listeners to notify
            signalWithNoParameter.Dispatch();
            Assert.False(callbackReceived);

            // null object to attach
            signalWithNoParameter.AddListener(null, callback);
            signalWithNoParameter.Dispatch();
            Assert.False(callbackReceived);
            
            // expected usage
            signalWithNoParameter.AddListener(_gameObjectToAttach, otherCallback);
            Assert.False(otherCallbackReceived);
            // notify callbacks
            signalWithNoParameter.Dispatch();
            Assert.True(otherCallbackReceived);
            
            callbackReceived = false;
            otherCallbackReceived = false;
            
            // remove and add again
            signalWithNoParameter.RemoveListener(_gameObjectToAttach);
            signalWithNoParameter.AddListener(_gameObjectToAttach, otherCallback);
            // notify callbacks
            signalWithNoParameter.Dispatch();
            Assert.True(otherCallbackReceived);
            
            otherCallbackReceived = false;
            
            // Add another to same object
            Assert.Throws<SignalAlreadyAttachedException>(() =>
            {
                signalWithNoParameter.AddListener(_gameObjectToAttach, callback);
            });
        }
        
        [Test]
        public void TestSignalWithOneParameter()
        {
            var signalWithOneParameters = _signalHub.Get<TestSignalOneParameter>();
            Assert.NotNull(signalWithOneParameters);
            
            var callback = new Action<TestParameterType>((param) =>
            {
                Assert.NotNull(param);
                Assert.AreEqual(param.Id, 10);
            });
            var otherCallback = new Action<TestParameterType>((param) =>
            {
                Assert.NotNull(param);
                Assert.AreEqual(param.Id, 10);
            });
            
            // No listeners to remove
            signalWithOneParameters.RemoveListener(_gameObjectToAttach);
            // No listeners to notify
            signalWithOneParameters.Dispatch(new TestParameterType(5));

            // null object to attach
            signalWithOneParameters.AddListener(null, callback);

            // expected usage
            signalWithOneParameters.AddListener(_gameObjectToAttach, otherCallback);
            // notify callbacks
            signalWithOneParameters.Dispatch(new TestParameterType(10));
            
            // remove and add again
            signalWithOneParameters.RemoveListener(_gameObjectToAttach);
            signalWithOneParameters.AddListener(_gameObjectToAttach, otherCallback);
            
            // Add another to same object
            Assert.Throws<SignalAlreadyAttachedException>(() =>
            {
                signalWithOneParameters.AddListener(_gameObjectToAttach, callback);
            });
        }
        
        [Test]
        public void TestSignalWithTwoParameters()
        {
            var signalWithTwoParameters = _signalHub.Get<TestSignalTwoParameters>();
            Assert.NotNull(signalWithTwoParameters);
            
            var callback = new Action<TestParameterType, int>((param1, param2) =>
            {
                Assert.NotNull(param1);
                Assert.NotNull(param2);
                Assert.AreEqual(param1.Id, 50);
                Assert.AreEqual(param2, 100);
            });
            var otherCallback = new Action<TestParameterType, int>((param1, param2) =>
            {
                Assert.NotNull(param1);
                Assert.NotNull(param2);
                Assert.AreEqual(param1.Id, 50);
                Assert.AreEqual(param2, 100);
            });
            
            // No listeners to remove
            signalWithTwoParameters.RemoveListener(_gameObjectToAttach);
            // No listeners to notify
            signalWithTwoParameters.Dispatch(new TestParameterType(5), 2);

            // null object to attach
            signalWithTwoParameters.AddListener(null, callback);

            // expected usage
            signalWithTwoParameters.AddListener(_gameObjectToAttach, otherCallback);
            // notify callbacks
            signalWithTwoParameters.Dispatch(new TestParameterType(50), 100);
            
            // remove and add again
            signalWithTwoParameters.RemoveListener(_gameObjectToAttach);
            signalWithTwoParameters.AddListener(_gameObjectToAttach, otherCallback);
            
            // Add another to same object
            Assert.Throws<SignalAlreadyAttachedException>(() =>
            {
                signalWithTwoParameters.AddListener(_gameObjectToAttach, callback);
            });
        }
        
        [Test]
        public void TestSignalWithThreeParameters()
        {
            var signalWithThreeParameters = _signalHub.Get<TestSignalThreeParameters>();
            Assert.NotNull(signalWithThreeParameters);
            
            var callback = new Action<TestParameterType, int, string>((param1, param2, param3) =>
            {
                Assert.NotNull(param1);
                Assert.NotNull(param2);
                Assert.NotNull(param3);
                Assert.AreEqual(param1.Id, 50);
                Assert.AreEqual(param2, 100);
                Assert.AreEqual(param3, "b");
            });
            var otherCallback = new Action<TestParameterType, int, string>((param1, param2, param3) =>
            {
                Assert.NotNull(param1);
                Assert.NotNull(param2);
                Assert.NotNull(param3);
                Assert.AreEqual(param1.Id, 50);
                Assert.AreEqual(param2, 100);
                Assert.AreEqual(param3, "b");
            });
            
            // No listeners to remove
            signalWithThreeParameters.RemoveListener(_gameObjectToAttach);
            // No listeners to notify
            signalWithThreeParameters.Dispatch(new TestParameterType(5), 2, "a");

            // null object to attach
            signalWithThreeParameters.AddListener(null, callback);

            // expected usage
            signalWithThreeParameters.AddListener(_gameObjectToAttach, otherCallback);
            // notify callbacks
            signalWithThreeParameters.Dispatch(new TestParameterType(50), 100, "b");
            
            // remove and add again
            signalWithThreeParameters.RemoveListener(_gameObjectToAttach);
            signalWithThreeParameters.AddListener(_gameObjectToAttach, otherCallback);
            
            // Add another to same object
            Assert.Throws<SignalAlreadyAttachedException>(() =>
            {
                signalWithThreeParameters.AddListener(_gameObjectToAttach, callback);
            });
        }
        
        [Test]
        public void TestRemoveAll()
        {
            var signalWithNoParameter = _signalHub.Get<TestSignalNoParameter>();
            Assert.NotNull(signalWithNoParameter);
            signalWithNoParameter.AddListener(_gameObjectToAttach, () =>
            {
                Assert.Fail();
            });
            
            var signalWithOneParameters = _signalHub.Get<TestSignalOneParameter>();
            Assert.NotNull(signalWithOneParameters);
            signalWithOneParameters.AddListener(_gameObjectToAttach, (param) =>
            {
                Assert.Fail();
            });
            
            var signalWithTwoParameters = _signalHub.Get<TestSignalTwoParameters>();
            Assert.NotNull(signalWithTwoParameters);
            signalWithTwoParameters.AddListener(_gameObjectToAttach, (param1, param2) =>
            {
                Assert.Fail();
            });
            
            var signalWithThreeParameters = _signalHub.Get<TestSignalThreeParameters>();
            Assert.NotNull(signalWithThreeParameters);
            signalWithThreeParameters.AddListener(_gameObjectToAttach, (param1, param2, param3) =>
            {
                Assert.Fail();
            });
            
            _signalHub.RemoveAllListeners(_gameObjectToAttach);
            
            signalWithNoParameter.Dispatch();
            signalWithOneParameters.Dispatch(new TestParameterType(1));
            signalWithTwoParameters.Dispatch(new TestParameterType(2), 3);
            signalWithThreeParameters.Dispatch(new TestParameterType(3), 4, "5");
        }

        [Test]
        public void TestRemoveAllForOneObject()
        {
            var signalWithNoParameter = _signalHub.Get<TestSignalNoParameter>();
            Assert.NotNull(signalWithNoParameter);
            signalWithNoParameter.AddListener(_gameObjectToAttach, () =>
            {
                Assert.Fail();
            });

            GameObject secondObject = new GameObject();
            secondObject.name = "ABC";
 
            signalWithNoParameter.AddListener(secondObject, () =>
            {
                secondObject.name = "XYZ";
            });
            
            _signalHub.RemoveAllListeners(_gameObjectToAttach);
            
            signalWithNoParameter.Dispatch();
            Assert.AreEqual("XYZ", secondObject.name);

        }

        [TearDown]
        public void TearDown()
        {
            _signalHub.RemoveAllListeners(_gameObjectToAttach);
            Object.DestroyImmediate(_gameObjectToAttach);
        }
    }

    public class TestParameterType
    {
        public int Id;

        public TestParameterType(int id)
        {
            Id = id;
        }
    }
    
    public class TestSignalNoParameter: ASignal { }
    public class TestSignalOneParameter: ASignal<TestParameterType> { }
    public class TestSignalTwoParameters: ASignal<TestParameterType, int> { }
    public class TestSignalThreeParameters: ASignal<TestParameterType, int, string> { }
}