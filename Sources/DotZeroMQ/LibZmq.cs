using System;
using System.Runtime.InteropServices;

namespace DotZeroMQ
{
    internal static class LibZmq
    {
        private const string Lib = "libzmq";
        private const CallingConvention CallingConv = CallingConvention.Cdecl;

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern ZmqContextSafeHandle zmq_ctx_new();

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_ctx_term(IntPtr context);

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern ZmqSocketSafeHandle zmq_socket(ZmqContextSafeHandle context, ZmqSocketType type);

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_close(IntPtr socket);
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_bind(ZmqSocketSafeHandle socket, string endpoint);

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_unbind(ZmqSocketSafeHandle socket, string endpoint);

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_connect(ZmqSocketSafeHandle socket, string endpoint);

        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_disconnect(ZmqSocketSafeHandle socket, string endpoint);
    }
}
