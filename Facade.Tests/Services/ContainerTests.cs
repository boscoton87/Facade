using NUnit.Framework;
using Facade.Services;
using Facade.Tests.Mocks.Interfaces;
using Facade.Tests.Mocks.Services;
using System;

namespace Facade.Tests.Services
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void AssertThrowsIfMappingNotInterface()
        {
            Assert.Throws<Exception>(() => Container.RegisterGlobalInstance<Counter>(new Counter(string.Empty)), $"{nameof(Counter)} is not an Interface.");
        }

        [Test]
        public void AssertThrowsIfInstanceDoesNotImplementInterface()
        {
            Assert.Throws<Exception>(() => Container.RegisterGlobalInstance<IOtherService>(new Counter(string.Empty)), 
                $"Instance parameter provided does not implement the {nameof(IOtherService)} interface.");
        }

        [Test]
        public void AssertThrowsIfInterfaceAlreadyMappedToInstance()
        {
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(string.Empty));
            Assert.Throws<Exception>(() => container.RegisterInstance<ICounter>(new Counter(string.Empty)), $"hello world");
        }

        [Test]
        public void AssertCanRegisterInstanceGlobal()
        {
            string servicename = "Counter";
            Container.RegisterGlobalInstance<ICounter>(new Counter(servicename));
            ICounter counter = Container.ResolveGlobalInstance<ICounter>();
            Assert.AreEqual($"{servicename}: 0", counter.GetStatus());

            Container.RemoveGlobalInstanceMapping<ICounter>();
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

        [Test]
        public void AssertInstanceCanOverride()
        {
            string globalServiceName = "Countera";
            Container.RegisterGlobalInstance<ICounter>(new Counter(globalServiceName));

            string servicename = "Counterb";
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(servicename));
            ICounter counter = container.ResolveInstance<ICounter>();
            Assert.AreEqual($"{servicename}: 0", counter.GetStatus());

            Container.RemoveGlobalInstanceMapping<ICounter>();
        }
    } 
}
