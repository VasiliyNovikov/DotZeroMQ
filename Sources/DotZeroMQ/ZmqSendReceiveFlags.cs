using System;

namespace DotZeroMQ
{
    [Flags]
    public enum ZmqSendReceiveFlags
    {
        None = 0,
        DontWait = 1,
        SendMore = 2
    }
}