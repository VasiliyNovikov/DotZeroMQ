using System;
using System.Runtime.InteropServices;

namespace DotZeroMQ
{
    public class ZmqMessage : IDisposable
    {
        private IntPtr _nativeMessage;

        public int Size
        {
            get
            {
                CheckDisposed();
                return (int) LibZmq.zmq_msg_size(_nativeMessage).ToUInt32();
            }
        }

        public bool HasMore
        {
            get
            {
                this.CheckDisposed();
                return LibZmq.zmq_msg_more(_nativeMessage).ThrowIfLastError() != 0;
            }
        }

        public ZmqMessage(int initialSize = 0)
        {
            if (initialSize < 0) throw new ArgumentOutOfRangeException(nameof(initialSize));
            
            _nativeMessage = Marshal.AllocHGlobal(Marshal.SizeOf<LibZmq.zmq_msg_t>());
            if (initialSize == 0)
                LibZmq.zmq_msg_init(_nativeMessage).ThrowIfLastError();
            else
                LibZmq.zmq_msg_init_size(_nativeMessage, new UIntPtr((uint)initialSize)).ThrowIfLastError();
        }

        public ZmqMessage(byte[] data)
            : this(data.Length) => this.CopyFrom(data);

        public void Dispose()
        {
            CheckDisposed();
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ZmqMessage() => ReleaseUnmanagedResources();

        private void ReleaseUnmanagedResources()
        {
            try
            {
                LibZmq.zmq_msg_close(_nativeMessage).ThrowIfLastError();
            }
            finally
            {
                Marshal.FreeHGlobal(_nativeMessage);
                _nativeMessage = IntPtr.Zero;
            }
        }

        private void CheckDisposed()
        {
            if (_nativeMessage == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(ZmqMessage));
        }
        
        public IntPtr DangerousGetNativeMessage()
        {
            CheckDisposed();
            return _nativeMessage;
        }

        public IntPtr DangerousGetData()
        {
            CheckDisposed();
            return LibZmq.zmq_msg_data(_nativeMessage);
        }
    }
}