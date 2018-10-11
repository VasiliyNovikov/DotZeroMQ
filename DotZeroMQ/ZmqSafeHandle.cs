using System;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace DotZeroMQ
{
    public abstract class ZmqSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected ZmqSafeHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return this.ReleaseZmqObject(this.handle) == 0;
        }

        protected abstract int ReleaseZmqObject(IntPtr nativeZmqObject);
    }
}