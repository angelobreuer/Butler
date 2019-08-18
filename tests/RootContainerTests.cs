namespace Butler.Tests
{
    using System;
    using Butler.Tests.Dummy;
    using Butler.Util;
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
        public void TestTransientRegistration()
        {
            Container.Register<IDummyService, DummyService>().AsTransient();

            var dummyService1 = Container.Resolve<IDummyService>();
            var dummyService2 = Container.Resolve<IDummyService>();

            Assert.False(ReferenceEquals(dummyService1, dummyService2),
                "Expected that transient services do not have the same reference.");
        }

        [Fact]
        public void TestTrackingScoped()
        {
            Container.Register<DummyDisposeTracker, DummyDisposeTracker>().AsScoped();

            var dummyService1 = Container.Resolve<DummyDisposeTracker>(scopeKey: null);
            var dummyService2 = Container.Resolve<DummyDisposeTracker>(scopeKey: new object());

            Assert.False(ReferenceEquals(dummyService1, dummyService2),
                "Expected that resolving using different scopes does not returns the same service.");

            Assert.NotNull(dummyService1);
            Assert.NotNull(dummyService2);

            Container.Dispose();

            Assert.True(dummyService1.IsDisposed, "Expected that scoped service is disposed when disposing container (global scope).");
            Assert.True(dummyService2.IsDisposed, "Expected that scoped service is disposed when disposing container (non-global scope).");
        }

        [Fact]
        public void TestScopedRegistration()
        {
            var globalScope = default(object);
            var myScope = new object();

            Container.Register<IDummyService, DummyService>().AsScoped();

            var dummyService1 = Container.Resolve<IDummyService>(globalScope);
            var dummyService2 = Container.Resolve<IDummyService>(globalScope);
            var dummyService3 = Container.Resolve<IDummyService>(myScope);
            var dummyService4 = Container.Resolve<IDummyService>(myScope);

            Assert.NotNull(dummyService1);
            Assert.NotNull(dummyService2);
            Assert.NotNull(dummyService3);
            Assert.NotNull(dummyService4);

            Assert.True(ReferenceEquals(dummyService1, dummyService2),
                "Expected that resolving using the same scope returns the same service. (global scope)");

            Assert.True(ReferenceEquals(dummyService3, dummyService4),
                "Expected that resolving using the same scope returns the same service. (non-global scope)");

            Assert.False(ReferenceEquals(dummyService1, dummyService3),
                "Expected that resolving using different scopes does not returns the same service.");

            Assert.False(ReferenceEquals(dummyService2, dummyService4),
                "Expected that resolving using different scopes does not returns the same service.");

            Assert.False(ReferenceEquals(dummyService1, dummyService4),
                "Expected that resolving using different scopes does not returns the same service.");

            Assert.False(ReferenceEquals(dummyService2, dummyService3),
                "Expected that resolving using different scopes does not returns the same service.");
        }

        [Fact]
        public void TestTrackingTransient()
        {
            Container.RegisterDirect<DummyDisposeTracker, DummyDisposeTracker>();

            var disposeTracker = Container.Resolve<DummyDisposeTracker>();

            Assert.False(disposeTracker.IsDisposed);

            Container.Dispose();

            Assert.True(disposeTracker.IsDisposed);
        }

        [Fact]
        public void TestTryResolveUnregisteredService()
        {
            Assert.Throws<ResolverException>(() => Container.Resolve<int>());
        }

        /// <summary>
        ///     Gets the container being tested.
        /// </summary>
        public IRootContainer Container { get; }

        /// <summary>
        ///     Tests resolving a <see langword="null"/> service.
        /// </summary>
        [Fact]
        public void TestNullResolve() => Assert.Throws<ArgumentNullException>(() => Container.Resolve(null));
    }
}