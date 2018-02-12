namespace DotZeroMQ
{
    public static class ZmqContextExtensions
    {
        public static ZmqSocket Socket(this ZmqContext context, ZmqSocketType socketType)
        {
            return new ZmqSocket(context, socketType);
        }
    }
}