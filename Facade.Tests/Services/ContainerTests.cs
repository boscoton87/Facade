using NUnit.Framework;
using Facade.Services;
using Facade.Tests.Mocks.Interfaces;
using Facade.Tests.Mocks.Services;
using System;
using Facade.Exceptions;

namespace Facade.Tests.Services
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void AssertThrowsIfInstanceDoesNotImplementInterface()
        {
            Assert.Throws<InvalidTypeMappingException>(() => Container.RegisterGlobalInstance<IOtherService>(new Counter(string.Empty)), 
                $"Instance parameter provided does not implement the {nameof(IOtherService)} interface.");
        }

        [Test]
        public void AssertThrowsIfInterfaceAlreadyMappedToInstance()
        {
            Container container = new Container();
            container.RegisterInstance<ICounter>(new Counter(string.Empty));
            Assert.Throws<MappingTakenException>(() => container.RegisterInstance<ICounter>(new Counter(string.Empty)), $"hello world");
        }

        [Test]
        public void AssertThrowsIfInstanceHasNoMapping()
        {
            Assert.Throws<NoMappingException>(() => Container.ResolveGlobalInstance<ICounter>());
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
        public void AssertCanRegisterInstanceNonInterfaceGlobal()
        {
            string serviceName = "Counter";
            Container.RegisterGlobalInstance<Counter>(new Counter(serviceName));
            var counter = Container.ResolveGlobalInstance<Counter>();
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());

            Container.RemoveGlobalInstanceMapping<Counter>();
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
        public void AssertThrowsIfTypeDoesNotImplementInterface()
        {
            Assert.Throws<InvalidTypeMappingException>(() => Container.RegisterGlobalType<IOtherService, Counter>(),
                $"Counter does not implement {nameof(IOtherService)}.");
        }

        [Test]
        public void AssertThrowsIfTypeDoesNotHaveConstructorWithParameterTypes()
        {
            Assert.Throws<InvalidTypeMappingException>(() => Container.RegisterGlobalType<IOtherService, Counter>(5),
                $"Counter does not have a constructor that accepts the supplied parameters.");
        }

        [Test]
        public void AssertThrowsIfTypeHasNoMapping()
        {
            Assert.Throws<NoMappingException>(() => Container.ResolveGlobalType<ICounter>());
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
        public void AssertCanRegisterNamedType()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterNamedType<ICounter, Counter>("foo", serviceName);
            ICounter counter = container.ResolveNamedType<ICounter>("foo");
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());
        }

        [Test]
        public void AssertCanRegisterMultipleNamedTypes()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterNamedType<ICounter, Counter>("foo", serviceName);
            container.RegisterNamedType<ICounter, Counter>("bar", serviceName);
            ICounter fooCounter = container.ResolveNamedType<ICounter>("foo");
            ICounter barCounter = container.ResolveNamedType<ICounter>("bar");
            barCounter.Increment();
            Assert.AreEqual($"{serviceName}: 0", fooCounter.GetStatus());
            Assert.AreEqual($"{serviceName}: 1", barCounter.GetStatus());
        }

        [Test]
        public void AssertCanRegisterNamedInstance()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterNamedInstance<ICounter>("foo", new Counter(serviceName));
            ICounter counter = container.ResolveNamedInstance<ICounter>("foo");
            Assert.AreEqual($"{serviceName}: 0", counter.GetStatus());
        }

        [Test]
        public void AssertCanRegisterMultipleNamedInstances()
        {
            string serviceName = "Counter";
            Container container = new Container();
            container.RegisterNamedInstance<ICounter>("foo", new Counter(serviceName));
            container.RegisterNamedInstance<ICounter>("bar", new AdvancedCounter(serviceName, 2));
            ICounter fooCounter = container.ResolveNamedInstance<ICounter>("foo");
            ICounter barCounter = container.ResolveNamedInstance<ICounter>("bar");
            barCounter.Increment();
            Assert.AreEqual($"{serviceName}: 0", fooCounter.GetStatus());
            Assert.AreEqual($"{serviceName}: 2", barCounter.GetStatus());
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

        [Test]
        public void AssertCanRegisterInheritedType()
        {
            Container container = new Container();
            container.RegisterType<Counter, AdvancedCounter>("counter", 2);
            var counter = container.ResolveType<Counter>();
            counter.Increment();
            Assert.AreEqual("counter: 2", counter.GetStatus());
        }

        [Test]
        public void AssertCanRegisterInheritedInstance()
        {
            Container container = new Container();
            var counter = new AdvancedCounter("counter", 2);
            container.RegisterInstance<Counter>(counter);
            var resultCounter = container.ResolveInstance<Counter>();
            resultCounter.Increment();
            Assert.AreEqual("counter: 2", resultCounter.GetStatus());
        }

        [Test]
        public void AssertThrowsWhenRegisterMethodMethodInfoIsNull()
        {
            string methodName = "PrintAge";
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Assert.Throws<ArgumentNullException>(() => Container.RegisterGlobalMethod(methodName, null, method.Target));
        }

        [Test]
        public void AssertThrowsWhenRegisterMethodMethodKeyIsEmpty()
        {
            string methodName = string.Empty;
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Assert.Throws<InvalidArgumentException>(() => Container.RegisterGlobalMethod(methodName, method.Method, method.Target));
        }

        [Test]
        public void AssertThrowsWhenRegisterMethodMethodKeyIsNull()
        {
            string methodName = null;
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Assert.Throws<InvalidArgumentException>(() => Container.RegisterGlobalMethod(methodName, method.Method, method.Target));
        }

        [Test]
        public void AssertThrowsWhenRegisterMethodMethodAlreadyMapped()
        {
            string methodName = "PrintAge";
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Container.RegisterGlobalMethod(methodName, method.Method, method.Target);
            Assert.Throws<MappingTakenException>(() => Container.RegisterGlobalMethod(methodName, method.Method, method.Target));

            Container.RemoveGlobalMethodMapping(methodName);
        }

        [Test]
        public void AssertThrowsIfMethodHasNoMapping()
        {
            string methodName = "PrintAge";
            Assert.Throws<NoMappingException>(() => Container.InvokeGlobalMethod(methodName));
        }

        [Test]
        public void AssertThrowsIfMethodHasNoMappingWithSpecifiedParameterTypes()
        {
            string methodName = "PrintAge";
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Container.RegisterGlobalMethod(methodName, method.Method, method.Target);
            Assert.Throws<MethodInvocationException>(() => Container.InvokeGlobalMethod(methodName, "Alex", "Test"));

            Container.RemoveGlobalMethodMapping(methodName);
        }

        [Test]
        public void AssertCanRegisterMethodGlobal()
        {
            string methodName = "PrintAge";
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            Container.RegisterGlobalMethod(methodName, method.Method, method.Target);
            Assert.AreEqual("Alex is 27 years old!", Container.InvokeGlobalMethod(methodName, "Alex", 27));

            Container.RemoveGlobalMethodMapping(methodName);
        }

        [Test]
        public void AssertCanRegisterMethod()
        {
            string methodName = "PrintAge";
            var method = new Func<string, int?, string>((name, age) => $"{name} is {age} years old!");
            var container = new Container();
            container.RegisterMethod(methodName, method.Method, method.Target);

            Assert.AreEqual("Alex is 27 years old!", container.InvokeMethod(methodName, "Alex", 27));
        }

        [Test]
        public void AssertMethodCanOverride()
        {
            string methodName = "PrintAge";

            var globalMethod = new Func<string, int?, string>((name, age) => $"{name} is {age} years old");
            Container.RegisterGlobalMethod(methodName, globalMethod.Method, globalMethod.Target);

            var container = new Container();
            var method = new Func<string, int?, string>((name, age) => $"{name} is also {age} years old!");
            container.RegisterMethod(methodName, method.Method, method.Target);

            Assert.AreEqual("Alex is also 27 years old!", container.InvokeMethod(methodName, "Alex", 27));

            Container.RemoveGlobalMethodMapping(methodName);
        }
    } 
}
