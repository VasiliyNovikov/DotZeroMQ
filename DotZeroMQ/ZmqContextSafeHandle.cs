using System;

namespace DotZeroMQ
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ZmqContextSafeHandle : ZmqSafeHandle
    {
        protected override int ReleaseZmqObject(IntPtr context)
        {
            return LibZmq.zmq_ctx_term(context);
        }
    }
}