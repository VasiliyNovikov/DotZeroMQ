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

        public void Bind(string endPoint)
        {
            LibZmq.zmq_bind(this.Handle, endPoint).ThrowIfLastError();
        }
        
        public void Unbind(string endPoint)
        {
            LibZmq.zmq_unbind(this.Handle, endPoint).ThrowIfLastError();
        }
        
        public void Connect(string endPoint)
        {
            LibZmq.zmq_connect(this.Handle, endPoint).ThrowIfLastError();
        }

        public void Disconnect(string endPoint)
        {
            LibZmq.zmq_disconnect(this.Handle, endPoint).ThrowIfLastError();
        }
    }
}