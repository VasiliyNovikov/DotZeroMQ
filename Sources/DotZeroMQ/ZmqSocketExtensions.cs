using System;

namespace DotZeroMQ
{
    public static class ZmqSocketExtensions
    {
        public static void Send(this ZmqSocket socket, byte[] message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var msg = new ZmqMessage(message))
                socket.Send(msg, flags);
        }
        
        public static byte[] Receive(this ZmqSocket socket, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            using (var result = new ZmqMessage())
            {
                socket.Receive(result, flags);
                return result.ToArray();
            }
        }
    }
}