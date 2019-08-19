namespace Butler.Tests
{
    using System;
    using Butler.Tests.Dummy;
    using Butler.Util;
    using Butler.Lifetime;
    using Butler.Resolver;
    using Butler.Register;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="RootContainer"/> class.
    /// </summary>
    public sealed class RootContainerTests
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RootContainerTests"/> class.
        /// </summary>
        public RootContainerTests()
            => Container = new RootContainer();

        [Fact]
        public void TestMultiRegistration()
        {
            var firstService = new DummyService();
            var secondService = new OtherDummyService();

            Container.RegisterInstance<IDummyService>(firstService, registrationMode: ServiceRegistrationMode.Append);
            Container.RegisterInstance<IDummyService>(secondService, registrationMode: ServiceRegistrationMode.Append);

            var services = Container.ResolveAll<IDummyService>();

            Assert.NotNull(services);
            Assert.Contains(firstService, services);
            Assert.Contains(secondService, services);
        }

        [Fact]
        public void TestTryResolveUnregisteredService()
            => Assert.Throws<ResolverException>(() => Container.Resolve<int>());

        /// <summary>
        ///     Gets the container being tested.
        /// </summary>
        public IRootContainer Container { get; }

        /// <summary>
        ///     Tests resolving a <see langword="null"/> service.
        /// </summary>
        [Fact]
        public void TestNullResolve() => Assert.Throws<ArgumentNullException>(() => Container.Resolve(null));

        [Fact]
        public void TestResolveUnregisteredWithExceptionMode()
            => Assert.Throws<ResolverException>(
                () => Container.Resolve<IDummyService>(
                    resolveMode: ServiceResolveMode.ThrowException));

        [Fact]
        public void TestresolveUnregisteredWithReturnDefaultMode()
            => Assert.Null(Container.Resolve<IDummyService>(
                resolveMode: ServiceResolveMode.ReturnDefault));

        /// <summary>
        ///     Tests the basic resolution of a service.
        /// </summary>
        [Fact]
        public void TestBasicResolve()
        {
            Container.Register<IDummyService, DummyService>(Lifetime.Singleton);

            var service = Container.Resolve(typeof(IDummyService));

            Assert.NotNull(service);
            Assert.IsType<DummyService>(service);
        }
    }
}