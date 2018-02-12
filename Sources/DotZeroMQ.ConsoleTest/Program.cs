using System;
using System.Threading.Tasks;

namespace DotZeroMQ.ConsoleTest
{
    internal static class Program
    {
        private static async Task Main()
        {
            var endpoint = $"ipc:///tmp/{Guid.NewGuid():N}.sock";
            await Task.WhenAll(Task.Run(() => Server(endpoint)), Task.Run(() => Client(endpoint)));
        }

        private static void Server(string endpoint)
        {
            using (var server = ZmqContext.Current.Socket(ZmqSocketType.Rep))
            {
                server.Bind(endpoint);
                
                Console.WriteLine($"Server received: {server.ReceiveText()}");
                var message = "World";
                server.SendText(message);
                Console.WriteLine($"Server sent: {message}");
            }

        }

        private static void Client(string endpoint)
        {
            using (var clientContext = new ZmqContext())
            using (var client = clientContext.Socket(ZmqSocketType.Req))
            {
                client.Connect(endpoint);
                
                const string message = "Hello";
                client.SendText(message);
                Console.WriteLine($"Client sent: {message}");
                Console.WriteLine($"Client received: {client.ReceiveText()}");
            }
        }
    }
}
