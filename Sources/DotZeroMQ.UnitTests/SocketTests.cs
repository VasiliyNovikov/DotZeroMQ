using System;
using System.Collections.Generic;
using System.Linq;
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
            yield return $"ipc:///tmp/{Guid.NewGuid():N}.sock";
            yield return $"inproc://{Guid.NewGuid():N}";
        }

        private static List<object[]> GetEndpointParam()
        {
            return GetEndpoints().Select(e => new object[] {e}).ToList();
        }

        private static IEnumerable<object[]> GetSocketTypeParam() => SocketTypes.Select(st => new object[]{st});

        private static List<object[]> GetSocketTypeAndEndpointParams()
        {
            return SocketTypes
                .SelectMany(st => GetEndpoints().ToList(), (st, addr) => new object[] {st, addr})
                .ToList();
        }
        
        private static IEnumerable<object[]> GetWrongEndpointsParam()
        {
            yield return new object[]{ "tpc://127.0.0.1:5556" };
            yield return new object[]{ "tcp://10.20.30.40:5556" };
            yield return new object[]{ "ipc:///nonexistentdir/test.sock" };
        }

        [TestMethod]
        [DynamicData(nameof(GetSocketTypeParam), DynamicDataSourceType.Method)]
        public void Socket_Create_Dispose_Test(ZmqSocketType socketType)
        {
            ZmqSocket socket;
            using (socket = new ZmqSocket(ZmqContext.Current, socketType))
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
            using (var socket = new ZmqSocket(ZmqContext.Current, socketType))
            {
                socket.Bind(endpoint);
                socket.Unbind(endpoint);
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetSocketTypeAndEndpointParams), DynamicDataSourceType.Method)]
        public void Socket_Connect_Disconnect_Test(ZmqSocketType socketType, string endpoint)
        {
            using (var socket = new ZmqSocket(ZmqContext.Current, socketType))
            {
                socket.Connect(endpoint);
                socket.Disconnect(endpoint);
            }
        }

        [TestMethod]
        [DynamicData(nameof(GetWrongEndpointsParam), DynamicDataSourceType.Method)]
        public void Socket_Bind_Wrong_Endpoint_Test(string endpoint)
        {
            using (var socket = new ZmqSocket(ZmqContext.Current, ZmqSocketType.Req))
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<ZmqException>(() => socket.Bind(endpoint));
        }

        [TestMethod]
        public void Socket_Connect_Wrong_Endpoint_Test()
        {
            using (var socket = new ZmqSocket(ZmqContext.Current, ZmqSocketType.Req))
                // ReSharper disable once AccessToDisposedClosure
                Assert.ThrowsException<ZmqException>(() => socket.Connect("tpc://127.0.0.1:5556"));
        }

        [TestMethod]
        [DynamicData(nameof(GetEndpointParam), DynamicDataSourceType.Method)]
        public void Socket_Send_Receive_Test(string endpoint)
        {
            using (var receiver = new ZmqSocket(ZmqContext.Current, ZmqSocketType.Rep))
            using (var sender = new ZmqSocket(ZmqContext.Current, ZmqSocketType.Req))
            {
                receiver.Bind(endpoint);
                sender.Connect(endpoint);
                var message = MessageTests.GetTestData();
                sender.Send(message);
                var receivedMessage = receiver.Receive();
                CollectionAssert.AreEqual(message, receivedMessage);
            }
        }
    }
}