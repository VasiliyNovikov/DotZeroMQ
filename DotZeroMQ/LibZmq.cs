using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DotZeroMQ
{
    internal static class LibZmq
    {
        static LibZmq()
        {
            string libName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                libName = "libzmq.so";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                libName = "libzmq.dylib";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                    libName = "libzmq-x64.dll";
                else
                    libName = "libzmq-Win32.dll";
            }
            else
                throw new NotSupportedException($"Unsupported platform {RuntimeInformation.OSDescription}");

            string libPath;
            using (var libResourceStream = typeof(LibZmq).Assembly.GetManifestResourceStream($"{typeof(LibZmq).Namespace}.lib.{libName}"))
            {
                using (var sha256Hash = SHA256.Create())
                {
                    var rawHash = sha256Hash.ComputeHash(libResourceStream);
                    var hash = new StringBuilder();
                    foreach (byte b in rawHash)
                        hash.Append(b.ToString("x2"));
                    libPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{libName}.{hash}");
                }

                if (!File.Exists(libPath))
                {
                    using var libStream = File.OpenWrite(libPath);
                    libResourceStream.Position = 0;
                    libResourceStream.CopyTo(libStream);
                }
            }

            var lib = new NativeLibrary(libPath);
            GC.SuppressFinalize(lib);

            zmq_ctx_new = lib.GetFunction<zmq_ctx_new_func>(nameof(zmq_ctx_new));
            zmq_ctx_term = lib.GetFunction<zmq_ctx_term_func>(nameof(zmq_ctx_term));

            zmq_msg_init = lib.GetFunction<zmq_msg_func>(nameof(zmq_msg_init));
            zmq_msg_init_size = lib.GetFunction<zmq_msg_init_size_func>(nameof(zmq_msg_init_size));
            zmq_msg_close = lib.GetFunction<zmq_msg_func>(nameof(zmq_msg_close));
            zmq_msg_size = lib.GetFunction<zmq_msg_size_func>(nameof(zmq_msg_size));
            zmq_msg_data = lib.GetFunction<zmq_msg_data_func>(nameof(zmq_msg_data));
            zmq_msg_more = lib.GetFunction<zmq_msg_func>(nameof(zmq_msg_more));

            zmq_socket = lib.GetFunction<zmq_socket_func>(nameof(zmq_socket));
            zmq_close = lib.GetFunction<zmq_socket_close_func>(nameof(zmq_close));
            zmq_bind = lib.GetFunction<zmq_socket_connection_func>(nameof(zmq_bind));
            zmq_unbind = lib.GetFunction<zmq_socket_connection_func>(nameof(zmq_unbind));
            zmq_connect = lib.GetFunction<zmq_socket_connection_func>(nameof(zmq_connect));
            zmq_disconnect = lib.GetFunction<zmq_socket_connection_func>(nameof(zmq_disconnect));
            zmq_msg_send = lib.GetFunction<zmq_msg_send_recv_func>(nameof(zmq_msg_send));
            zmq_msg_recv = lib.GetFunction<zmq_msg_send_recv_func>(nameof(zmq_msg_recv));
        }

        public static readonly zmq_ctx_new_func zmq_ctx_new;
        public static readonly zmq_ctx_term_func zmq_ctx_term;

        public static readonly zmq_msg_func zmq_msg_init;
        public static readonly zmq_msg_init_size_func zmq_msg_init_size;
        public static readonly zmq_msg_func zmq_msg_close;
        public static readonly zmq_msg_size_func zmq_msg_size;
        public static readonly zmq_msg_data_func zmq_msg_data;
        public static readonly zmq_msg_func zmq_msg_more;

        public static readonly zmq_socket_func zmq_socket;
        public static readonly zmq_socket_close_func zmq_close;
        public static readonly zmq_socket_connection_func zmq_bind;
        public static readonly zmq_socket_connection_func zmq_unbind;
        public static readonly zmq_socket_connection_func zmq_connect;
        public static readonly zmq_socket_connection_func zmq_disconnect;
        public static readonly zmq_msg_send_recv_func zmq_msg_send;
        public static readonly zmq_msg_send_recv_func zmq_msg_recv;

        #region Interop function signatures

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate ZmqContextSafeHandle zmq_ctx_new_func();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_ctx_term_func(IntPtr context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_msg_func(IntPtr msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_msg_init_size_func(IntPtr msg, UIntPtr size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate UIntPtr zmq_msg_size_func(IntPtr msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate IntPtr zmq_msg_data_func(IntPtr msg);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate ZmqSocketSafeHandle zmq_socket_func(ZmqContextSafeHandle context, ZmqSocketType type);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_socket_close_func(IntPtr socket);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_socket_connection_func(ZmqSocketSafeHandle socket, string endpoint);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        public delegate int zmq_msg_send_recv_func(IntPtr msg, ZmqSocketSafeHandle socket, ZmqSendReceiveFlags flags);

        #endregion Interop function signatures

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        public unsafe struct zmq_msg_t
        {
            private fixed byte _[64];
        }
    }
}
