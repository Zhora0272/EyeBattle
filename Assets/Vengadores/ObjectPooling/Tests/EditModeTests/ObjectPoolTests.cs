using NUnit.Framework;

namespace Vengadores.ObjectPooling.Tests.EditModeTests
{
    public class ObjectPoolTests
    {
        [Test]
        public void ObjectPoolTest()
        {
            var testPool = ObjectPool<TestModel>.CreatePool();

            var createdCount = 0;
            testPool.OnInstanceCreated += (o) => { createdCount++; };
            
            var poppedCount = 0;
            testPool.OnInstancePopped += (o) => { poppedCount++; };
            
            var pushedCount = 0;
            testPool.OnInstancePushed += (o) => { pushedCount++; };
            
            var disposedCount = 0;
            testPool.OnInstanceDisposed += (o) => { disposedCount++; };
            
            // Without param
            testPool.Allocate(2);
            Assert.AreEqual(createdCount, 2);

            var poppedInstance = testPool.Pop();
            Assert.NotNull(poppedInstance);
            Assert.AreEqual(poppedCount, 1);
            testPool.Pop();
            Assert.AreEqual(poppedCount, 2);
            
            // With param
            testPool.Allocate(2, "TestParam");
            Assert.AreEqual(createdCount, 4);
            
            var poppedInstanceWithParam = testPool.Pop();
            Assert.NotNull(poppedInstanceWithParam);
            Assert.AreEqual(poppedCount, 3);
            Assert.AreEqual(poppedInstanceWithParam.Param, "TestParam");
            testPool.Pop();
            Assert.AreEqual(poppedCount, 4);
            
            // Empty
            var newInstance = testPool.Pop(); // will allocate new without params
            Assert.AreNotEqual(newInstance.Param, "TestParam");
            Assert.AreEqual(createdCount, 5);
            Assert.AreEqual(poppedCount, 5);
            
            var newInstanceWithParam = testPool.Pop("TestParam2"); // will allocate new with params
            Assert.AreEqual(newInstanceWithParam.Param, "TestParam2");
            Assert.AreEqual(createdCount, 6);
            Assert.AreEqual(poppedCount, 6);
            
            // Push
            testPool.Push(poppedInstance);
            testPool.Push(poppedInstanceWithParam);
            testPool.Push(newInstance);
            testPool.Push(newInstanceWithParam);
            Assert.AreEqual(pushedCount, 4);
            
            testPool.DisposePool();
            Assert.AreEqual(disposedCount, 4);
        }
    }

    public class TestModel
    {
        public string Param;

        public TestModel()
        {
            
        }

        public TestModel(string param)
        {
            Param = param;
        }
    }
}