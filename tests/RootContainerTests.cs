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
        public void TestTrackingTransients()
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