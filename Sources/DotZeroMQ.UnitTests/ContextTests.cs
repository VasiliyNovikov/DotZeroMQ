using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotZeroMQ.UnitTests
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void Context_Create_Dispose_Test()
        {
            ZmqContext context;
            using (context = new ZmqContext())
            {
                Assert.IsFalse(context.Handle.IsClosed);
                Assert.IsFalse(context.Handle.IsInvalid);
            }
            
            Assert.IsTrue(context.Handle.IsClosed);
        }
    }
}