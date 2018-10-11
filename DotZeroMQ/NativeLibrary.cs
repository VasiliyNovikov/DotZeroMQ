using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace DotZeroMQ
{
    internal class NativeLibrary : IDisposable
    {
        private static readonly Implementation Impl;

        static NativeLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Impl = new WindowsImplementation();
            else
                Impl = new UnixImplementation();
        }

        private IntPtr _library;

        public NativeLibrary(string name)
        {
            _library = Impl.Load(name);
        }

        ~NativeLibrary()
        {
            ReleaseUnmanagedResources();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            if (_library == default)
                return;

            Impl.Unload(_library);
            _library = default;
        }

        public TFunction GetFunction<TFunction>(string name)
            where TFunction : Delegate
        {
            var functionPtr = Impl.GetFunction(_library, name);
            return Marshal.GetDelegateForFunctionPointer<TFunction>(functionPtr);
        }

        private abstract class Implementation
        {
            public abstract IntPtr Load(string name);
            public abstract void Unload(IntPtr library);
            public abstract IntPtr GetFunction(IntPtr library, string name);
        }

        private class WindowsImplementation : Implementation
        {
            private const string Kernel32 = nameof(Kernel32);

            [DllImport(Kernel32, SetLastError = true)]
            private static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport(Kernel32, SetLastError = true)]
            private static extern bool FreeLibrary(IntPtr hModule);

            [DllImport(Kernel32, SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            public override IntPtr Load(string name)
            {
                var library = LoadLibrary(name);
                if (library == default)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                return library;
            }

            public override void Unload(IntPtr library)
            {
                if (!FreeLibrary(library))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            public override IntPtr GetFunction(IntPtr library, string name)
            {
                var functionPtr = GetProcAddress(library, name);
                if (functionPtr == default)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                return functionPtr;
            }
        }

        private class UnixImplementation : Implementation
        {
            private const string Libdl = "libdl";

            [DllImport(Libdl)]
            private static extern IntPtr dlopen(string filename, int flag = 2);

            [DllImport(Libdl)]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);

            [DllImport(Libdl)]
            private static extern int dlclose(IntPtr handle);

            [DllImport(Libdl)]
            private static extern string dlerror();

            public override IntPtr Load(string name)
            {
                var library = dlopen(name);
                if (library == default)
                    throw new Win32Exception(dlerror());
                return library;
            }

            public override void Unload(IntPtr library)
            {
                if (dlclose(library) != 0)
                    throw new Win32Exception(dlerror());
            }

            public override IntPtr GetFunction(IntPtr library, string name)
            {
                var functionPtr = dlsym(library, name);
                if (functionPtr != default)
                    return functionPtr;

                dlerror();
                dlsym(library, name);
                var error = dlerror();
                if (error == null)
                    throw new Win32Exception($"Function symbol {name} is NULL");
                throw new Win32Exception(error);

            }
        }
    }
}
