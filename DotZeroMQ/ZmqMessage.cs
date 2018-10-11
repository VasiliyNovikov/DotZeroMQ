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
                this.CheckDisposed();
                return (int) LibZmq.zmq_msg_size(this._nativeMessage).ToUInt32();
            }
        }

        public bool HasMore
        {
            get
            {
                this.CheckDisposed();
                return LibZmq.zmq_msg_more(this._nativeMessage).ThrowIfLastError() != 0;
            }
        }

        public ZmqMessage(int initialSize = 0)
        {
            if (initialSize < 0) throw new ArgumentOutOfRangeException(nameof(initialSize));
            
            this._nativeMessage = Marshal.AllocHGlobal(Marshal.SizeOf<LibZmq.zmq_msg_t>());
            if (initialSize == 0)
                LibZmq.zmq_msg_init(this._nativeMessage).ThrowIfLastError();
            else
                LibZmq.zmq_msg_init_size(this._nativeMessage, new UIntPtr((uint)initialSize)).ThrowIfLastError();
        }

        public ZmqMessage(byte[] data)
            : this(data.Length)
        {
            this.CopyFrom(data);
        }

        public void Dispose()
        {
            this.CheckDisposed();
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~ZmqMessage()
        {
            this.ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            try
            {
                LibZmq.zmq_msg_close(this._nativeMessage).ThrowIfLastError();
            }
            finally
            {
                Marshal.FreeHGlobal(this._nativeMessage);
                this._nativeMessage = IntPtr.Zero;
            }
        }

        private void CheckDisposed()
        {
            if (this._nativeMessage == IntPtr.Zero)
                throw new ObjectDisposedException(nameof(ZmqMessage));
        }
        
        public IntPtr DangerousGetNativeMessage()
        {
            this.CheckDisposed();
            return this._nativeMessage;
        }

        public IntPtr DangerousGetData()
        {
            this.CheckDisposed();
            return LibZmq.zmq_msg_data(this._nativeMessage);
        }
    }
}