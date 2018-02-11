using System;

namespace DotZeroMQ
{
    public class ZmqContext : IDisposable
    {
        private static readonly Lazy<ZmqContext> CurrentLazy = new Lazy<ZmqContext>(true);

        public static ZmqContext Current => CurrentLazy.Value;
        
        public ZmqContextSafeHandle Handle { get; }
        
        public ZmqContext()
        {
            this.Handle = LibZmq.zmq_ctx_new().ThrowIfLastError();
        }
        
        public void Dispose()
        {
            this.Handle.Dispose();
        }

        public ZmqSocket Socket(ZmqSocketType socketType)
        {
            return new ZmqSocket(this, socketType);
        }
    }
}