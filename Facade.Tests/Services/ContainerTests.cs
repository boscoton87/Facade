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
            string serviceName = "Counter";
            Container.RegisterGlobalInstance<ICounter>(new Counter(serviceName));
            ICounter counter = Container.ResolveGlobalInstance<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());

            Container.RemoveGlobalInstanceMapping<ICounter>();
        }

        [Test]
        public void AssertCanRegisterInstance()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(serviceName));
            ICounter counter = container.ResolveInstance<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());
        }

        [Test]
        public void AssertInstanceCanOverride()
        {
            string globalServiceName = "Countera";
            Container.RegisterGlobalInstance<ICounter>(new Counter(globalServiceName));

            string serviceName = "Counterb";
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(serviceName));
            ICounter counter = container.ResolveInstance<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());

            Container.RemoveGlobalInstanceMapping<ICounter>();
        }

        [Test]
        public void AssertCanRegisterTypeGlobal()
        {
            string serviceName = "Counter";
            Container.RegisterGlobalType<ICounter, Counter>(serviceName);
            ICounter counter = Container.ResolveGlobalType<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());

            Container.RemoveGlobalTypeMapping<ICounter>();
        }

        [Test]
        public void AssertCanRegisterType()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterType<ICounter, Counter>(serviceName);
            ICounter counter = container.ResolveType<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());
        }

        [Test]
        public void AssertTypeCanOverride()
        {
            string globalServiceName = "Countera";
            Container.RegisterGlobalType<ICounter, Counter>(globalServiceName);

            string serviceName = "Counterb";
            Container container = new Container();
            container.RegisterType<ICounter, Counter>(serviceName);
            ICounter counter = container.ResolveType<ICounter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());

            Container.RemoveGlobalTypeMapping<ICounter>();
        }

        [Test]
        public void AssertCorrectConstructorCalledForInheritance()
        {
            Container container = new Container();
            container.RegisterType<ICounter, AdvancedCounter>("counter", 2);
            var counter = container.ResolveType<ICounter>();
            counter.Increment();
            Assert.AreEqual("counter: 2", counter.GetStatus());
        }
    } 
}
