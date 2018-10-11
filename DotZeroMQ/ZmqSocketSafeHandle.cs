using System;

namespace DotZeroMQ
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ZmqSocketSafeHandle : ZmqSafeHandle
    {
        protected override int ReleaseZmqObject(IntPtr socket)
        {
            return LibZmq.zmq_close(socket);
        }
    }
}