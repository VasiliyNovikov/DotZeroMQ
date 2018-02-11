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
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
            this.SocketType = socketType;
            this.Handle = LibZmq.zmq_socket(context.Handle, socketType).ThrowIfLastError();
        }

        public void Dispose()
        {
            this.Handle.Dispose();
        }

        private void CheckDisposed()
        {
            if (this.Handle.IsClosed)
                throw new ObjectDisposedException(nameof(ZmqSocket));
        }

        public void Bind(string endPoint)
        {
            this.CheckDisposed();
            LibZmq.zmq_bind(this.Handle, endPoint).ThrowIfLastError();
        }
        
        public void Unbind(string endPoint)
        {
            this.CheckDisposed();
            LibZmq.zmq_unbind(this.Handle, endPoint).ThrowIfLastError();
        }
        
        public void Connect(string endPoint)
        {
            this.CheckDisposed();
            LibZmq.zmq_connect(this.Handle, endPoint).ThrowIfLastError();
        }

        public void Disconnect(string endPoint)
        {
            this.CheckDisposed();
            LibZmq.zmq_disconnect(this.Handle, endPoint).ThrowIfLastError();
        }

        public void Send(ZmqMessage message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            this.CheckDisposed();
            LibZmq.zmq_msg_send(message.DangerousGetNativeMessage(), this.Handle, flags).ThrowIfLastError();
        }
        
        public void Send(byte[] message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            this.CheckDisposed();
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var msg = new ZmqMessage(message))
                this.Send(msg, flags);
        }
        
        public void Receive(ZmqMessage message, ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            this.CheckDisposed();
            if (message == null) throw new ArgumentNullException(nameof(message));
            
            LibZmq.zmq_msg_recv(message.DangerousGetNativeMessage(), this.Handle, flags).ThrowIfLastError();
        }

        public byte[] Receive(ZmqSendReceiveFlags flags = ZmqSendReceiveFlags.None)
        {
            this.CheckDisposed();
            var result = new ZmqMessage();
            this.Receive(result, flags);
            return result.ToArray();
        }
    }
}