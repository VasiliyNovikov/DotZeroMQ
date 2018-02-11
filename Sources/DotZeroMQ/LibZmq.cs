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
        

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        public unsafe struct zmq_msg_t
        {
            private fixed byte _[64];
        }
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_msg_init(IntPtr msg);
                
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_msg_init_size(IntPtr msg, UIntPtr size);
                
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_msg_close(IntPtr msg);
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern UIntPtr zmq_msg_size (IntPtr msg);
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern IntPtr zmq_msg_data (IntPtr msg);
        
        
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
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_msg_send (IntPtr msg, ZmqSocketSafeHandle socket, ZmqSendReceiveFlags flags);
        
        [DllImport(Lib, CallingConvention = CallingConv, SetLastError = true)]
        public static extern int zmq_msg_recv (IntPtr msg, ZmqSocketSafeHandle socket, ZmqSendReceiveFlags flags);
    }
}
