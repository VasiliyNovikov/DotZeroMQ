using System;
using System.Runtime.InteropServices;

namespace DotZeroMQ
{
    public static class ZmqMessageExtensions
    {
        public static void CopyTo(this ZmqMessage message, byte[] target, int targetOffset = 0, int length = -1)
        {
            if (targetOffset < 0 || targetOffset >= target.Length) throw new ArgumentOutOfRangeException(nameof(targetOffset));
            
            if (length == -1)
                length = target.Length;
            
            if (length <= 0 || targetOffset + length > target.Length || length > message.Size) throw new ArgumentOutOfRangeException(nameof(length));
            
            Marshal.Copy(message.DangerousGetData(), target, targetOffset, length);
        }

        public static void CopyFrom(this ZmqMessage message, byte[] source, int sourceOffset = 0, int length = -1)
        {
            if (sourceOffset < 0 || sourceOffset >= source.Length) throw new ArgumentOutOfRangeException(nameof(sourceOffset));
            
            if (length == -1)
                length = source.Length;
            
            if (length <= 0 || sourceOffset + length > source.Length || length > message.Size) throw new ArgumentOutOfRangeException(nameof(length));

            Marshal.Copy(source, sourceOffset, message.DangerousGetData(), length);
        }

        public static byte[] ToArray(this ZmqMessage message)
        {
            var result = new byte[message.Size];
            message.CopyTo(result);
            return result;
        }
    }
}