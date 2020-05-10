using System;

namespace DotZeroMQ
{
    public class ZmqSocket : IDisposable
    {
        public ZmqContext Context { get; }
        
        public ZmqSocketType SocketType { get; }
        
        public ZmqSocketSafeHandle Handle { get; } 

        public ZmqSocket(ZmqContext context, ZmqSocketType socketType)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            SocketType = socketType;
            Handle = LibZmq.zmq_socket(context.Handle, socketType).ThrowIfLastError();
        }

        public void Dispose() => Handle.Dispose();

        private void CheckDisposed()
        {
            if (Handle.IsClosed)
                throw new ObjectDisposedException(nameof(ZmqSocket));
        }

        public void Bind(string endPoint)
        {
            CheckDisposed();
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            LibZmq.zmq_bind(Handle, endPoint).ThrowIfLastError();
        }
        
        public void Unbind(string endPoint)
        {
            CheckDisposed();
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            LibZmq.zmq_unbind(Handle, endPoint).ThrowIfLastError();
        }
        
        public void Connect(string endPoint)
        {
            CheckDisposed();
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            LibZmq.zmq_connect(Handle, endPoint).ThrowIfLastError();
        }

        public void Disconnect(string endPoint)
        {
            CheckDisposed();
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));
            LibZmq.zmq_disconnect(Handle, endPoint).ThrowIfLastError();
        }

        public void Send(ZmqMessage message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            CheckDisposed();
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            LibZmq.zmq_msg_send(message.DangerousGetNativeMessage(), Handle, flags).ThrowIfLastError();
        }
        
        public void Receive(ZmqMessage message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            CheckDisposed();
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            LibZmq.zmq_msg_recv(message.DangerousGetNativeMessage(), Handle, flags).ThrowIfLastError();
        }
    }
}