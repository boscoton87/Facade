using NUnit.Framework;
using Facade.Services;
using Facade.Tests.Mocks.Interfaces;
using Facade.Tests.Mocks.Services;

namespace Facade.Tests.Services
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void AssertCanRegisterInstanceGlobal()
        {
            string servicename = "Counter";
            Container.RegisterGlobalInstance<ICounter>(new Counter(servicename));
            ICounter counter = Container.ResolveGlobalInstance<ICounter>();
            Assert.AreEqual($"{servicename}: 0", counter.GetStatus());
        }

        [Test]
        public void AssertCanRegisterInstance()
        {
            string servicename = "Counter";
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(servicename));
            ICounter counter = container.ResolveInstance<ICounter>();
            Assert.AreEqual($"{servicename}: 0", counter.GetStatus());
        }
    } 
}
