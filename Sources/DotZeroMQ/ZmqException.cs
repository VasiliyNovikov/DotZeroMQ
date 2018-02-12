using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DotZeroMQ
{
    public class ZmqException : Win32Exception
    {
        public ZmqException(int error) 
            : base(error)
        {
        }
    }

    public static class ZmqExceptionExtensions
    {
        public static int ThrowIfLastError(this int result)
        {
            if (result == -1)
                throw new ZmqException(Marshal.GetLastWin32Error());
            return result;
        }

        public static THandle ThrowIfLastError<THandle>(this THandle result)
            where THandle : SafeHandle
        {
            if (result == null || result.IsInvalid)
                throw new ZmqException(Marshal.GetLastWin32Error());
            return result;
        }
    }
}