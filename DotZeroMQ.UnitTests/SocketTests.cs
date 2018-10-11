using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotZeroMQ.UnitTests
{
    [TestClass]
    public class SocketTests
    {
        private static readonly ZmqSocketType[] SocketTypes = (ZmqSocketType[])Enum.GetValues(typeof(ZmqSocketType));

        private static int _testPortCounter = 1024;

        private static int GetTestPort()
        {
            ++_testPortCounter;
            if (_testPortCounter > 65535)
                _testPortCounter = 1025;
            return _testPortCounter;
        }
        
        private static IEnumerable<string> GetEndpoints()
        {
            yield return $"tcp://127.0.0.1:{GetTestPort()}";
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                yield return $"ipc:///tmp/{Guid.NewGuid():N}.sock";
            yield return $"inproc://{Guid.NewGuid():N}";
        }

        private static IEnumerable<object[]> GetEndpointParam()
        {
            return GetEndpoints().Select(e => new object[] {e});
        }

        private static IEnumerable<object[]> GetSocketTypeParam() => SocketTypes.Select(st => new object[]{st});

        private static IEnumerable<object[]> GetSocketTypeAndEndpointParams()
        {
            return SocketTypes.SelectMany(st => GetEndpoints(), (st, ep) => new object[] {st, ep});
        }
        
        private static IEnumerable<object[]> GetWrongEndpointsParam()
        {
            yield return new object[]{ "tpc://127.0.0.1:5556" };
            yield return new object[]{ "tcp://10.20.30.40:5556" };
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                yield return new object[]{ "ipc:///nonexistentdir/test.sock" };
        }

        [TestMethod]
        [DynamicData(nameof(GetSocketTypeParam), DynamicDataSourceType.Method)]
        public void Socket_Create_Dispose_Test(ZmqSocketType socketType)
        {
            ZmqSocket socket;
            using (socket = ZmqContext.Current.Socket(socketType))
            {
                Assert.AreEqual(ZmqContext.Current, socket.Context);
                Assert.AreEqual(socketType, socket.SocketType);
                Assert.IsFalse(socket.Handle.IsClosed);
                Assert.IsFalse(socket.Handle.IsInvalid);
            }
            
            Assert.IsTrue(socket.Handle.IsClosed);
        }
        
        [TestMethod]
        [DynamicData(nameof(GetSocketTypeAndEndpointParams), DynamicDataSourceType.Method)]
        public void Socket_Bind_Unbind_Test(ZmqSocketType socketType, string endpoint)
        {
            using (var socket = ZmqContext.Current.Socket(socketType))
            {
                socket.Bind(endpoint);
                socket.Unbind(endpoint);
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetSocketTypeAndEndpointParams), DynamicDataSourceType.Method)]
        public void Socket_Connect_Disconnect_Test(ZmqSocketType socketType, string endpoint)
        {
            using (var socket = ZmqContext.Current.Socket(socketType))
            {
                socket.Connect(endpoint);
                socket.Disconnect(endpoint);
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetWrongEndpointsParam), DynamicDataSourceType.Method)]
        public void Socket_Bind_Wrong_Endpoint_Test(string endpoint)
        {
            using (var socket = ZmqContext.Current.Socket(ZmqSocketType.Req))
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<ZmqException>(() => socket.Bind(endpoint));
        }

        [TestMethod]
        public void Socket_Connect_Wrong_Endpoint_Test()
        {
            using (var socket = ZmqContext.Current.Socket(ZmqSocketType.Req))
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<ZmqException>(() => socket.Connect("tpc://127.0.0.1:5556"));
        }

        [TestMethod]
        [DynamicData(nameof(GetEndpointParam), DynamicDataSourceType.Method)]
        public void Socket_Send_Receive_Test(string endpoint)
        {
            using (var receiver = ZmqContext.Current.Socket(ZmqSocketType.Rep))
            using (var senderContext = new ZmqContext())
            using (var sender = (endpoint.StartsWith("inproc") ? ZmqContext.Current : senderContext).Socket(ZmqSocketType.Req))
            {
                receiver.Bind(endpoint);
                sender.Connect(endpoint);
                
                var message = MessageTests.GetTestData();
                sender.Send(message);
                var receivedMessage = receiver.Receive();
                CollectionAssert.AreEqual(message, receivedMessage);
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetEndpointParam), DynamicDataSourceType.Method)]
        public void Socket_Send_Receive_Multipart_Test(string endpoint)
        {
            using (var receiver = new ZmqSocket(ZmqContext.Current, ZmqSocketType.Rep))
            using (var senderContext = new ZmqContext())
            using (var sender = (endpoint.StartsWith("inproc") ? ZmqContext.Current : senderContext).Socket(ZmqSocketType.Req))
            {
                receiver.Bind(endpoint);
                sender.Connect(endpoint);

                var message1 = MessageTests.GetTestData();
                var message2 = MessageTests.GetTestData();
                sender.Send(message1, ZmqSendReceiveFlags.SendMore);
                sender.Send(message2);

                using (var receivedMessage1 = new ZmqMessage())
                {
                    receiver.Receive(receivedMessage1);
                    CollectionAssert.AreEqual(message1, receivedMessage1.ToArray());
                    Assert.IsTrue(receivedMessage1.HasMore);
                }

                using (var receivedMessage2 = new ZmqMessage())
                {
                    receiver.Receive(receivedMessage2);
                    CollectionAssert.AreEqual(message2, receivedMessage2.ToArray());
                    Assert.IsFalse(receivedMessage2.HasMore);
                }
            }
        }
    }
}