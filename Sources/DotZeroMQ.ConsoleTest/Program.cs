using System;

namespace DotZeroMQ.ConsoleTest
{
    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Create context");
            using (var context = new ZmqContext())
            {
                var nativeContext = context.Handle.DangerousGetHandle();
                Console.WriteLine($"Context: {nativeContext}");
            }
            Console.WriteLine();


            Console.WriteLine("Create socket");
            using (var socket = ZmqContext.Current.Socket(ZmqSocketType.Rep))
            {
                var nativeSocket = socket.Handle.DangerousGetHandle();
                Console.WriteLine($"Socket: {nativeSocket}");
            }
            Console.WriteLine();
        }
    }
}
