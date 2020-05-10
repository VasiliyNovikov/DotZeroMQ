namespace DotZeroMQ
{
    public enum ZmqSocketType
    {
        Pair =      0x0,
        Pub =       0x1,
        Sub =       0x2,
        Req =       0x3,
        Rep =       0x4,
        Dealer =    0x5,
        Router =    0x6,
        Pull =      0x7,
        Push =      0x8,
        XPub =      0x9,
        XSub =      0xA,
        Stream =    0xB
    }
}