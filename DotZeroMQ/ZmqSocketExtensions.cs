using System;
using System.Text;

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
        
        public static void SendText(this ZmqSocket socket, string message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None, Encoding encoding = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var messageBytes = (encoding ?? Encoding.UTF8).GetBytes(message);
            socket.Send(messageBytes, flags);
        }
        
        public static byte[] Receive(this ZmqSocket socket, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            using (var result = new ZmqMessage())
            {
                socket.Receive(result, flags);
                return result.ToArray();
            }
        }

        public static unsafe string ReceiveText(this ZmqSocket socket, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None, Encoding encoding = null)
        {
            using (var result = new ZmqMessage())
            {
                socket.Receive(result, flags);
                return (encoding ?? Encoding.UTF8).GetString((byte*)result.DangerousGetData(), result.Size);
            }
        }
    }
}