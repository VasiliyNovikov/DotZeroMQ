using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotZeroMQ.UnitTests
{
    [TestClass]
    public class MessageTests
    {
        private const int TestMessageSize = 1024;

        public static byte[] GetTestData()
        {
            var testData = new byte[TestMessageSize];
            new Random().NextBytes(testData);
            return testData;
        }
        
        [TestMethod]
        public void Message_Empty_Create_Dispose_Test()
        {
            ZmqMessage message;
            using (message = new ZmqMessage())
                Assert.AreEqual(0, message.Size);

            Assert.ThrowsException<ObjectDisposedException>(() => message.Size);
        }
        
        [TestMethod]
        public void Message_WithSize_Create_Dispose_Test()
        {
            ZmqMessage message;
            using (message = new ZmqMessage(TestMessageSize))
            {
                Assert.AreEqual(TestMessageSize, message.Size);
                Assert.AreNotEqual(IntPtr.Zero, message.DangerousGetData());
            }

            Assert.ThrowsException<ObjectDisposedException>(() => message.Size);
        }
        
        [TestMethod]
        public void Message_Read_Write_Test()
        {
            var testData = GetTestData();
            using (var message = new ZmqMessage(TestMessageSize))
            {
                message.CopyFrom(testData);
                CollectionAssert.AreEqual(testData, message.ToArray());
            }
        }
    }
}