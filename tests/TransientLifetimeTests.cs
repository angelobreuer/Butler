namespace Butler.Tests
{
    using Butler.Lifetime;
    using Butler.Tests.Dummy;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="Lifetime.Transient"/> service lifetime.
    /// </summary>
    public sealed class TransientLifetimeTests
    {
        /// <summary>
        ///     Tests that transient registrations return the different services when resolving them.
        /// </summary>
        [Fact]
        public void TestTransientRegistration()
        {
            // create root container for test
            using (var container = new RootContainer())
            {
                // register the test service as transient
                container.Register<IDummyService, DummyService>().AsTransient();

                // create two different transient services
                var dummyService1 = container.Resolve<IDummyService>();
                var dummyService2 = container.Resolve<IDummyService>();

                // ensure the services are different creations
                Assert.False(ReferenceEquals(dummyService1, dummyService2),
                    "Expected that transient services do not have the same reference.");
            }
        }

        /// <summary>
        ///     Tests that tracked transient services are disposed after disposing the
        ///     <see cref="RootContainer"/> in the transient service lifetime.
        /// </summary>
        [Fact]
        public void TestTrackingTransient()
        {
            // create root container
            var container = new RootContainer();

            // register the dummy test service as transient
            container.Register<DummyDisposeTracker, DummyDisposeTracker>().AsTransient();

            // resolve tests services
            var service = container.Resolve<DummyDisposeTracker>();

            // dispose container after usage
            using (container)
            {
                // service null checks
                Assert.NotNull(service);

                // ensure the services are not disposed if the container is not
                Assert.False(service.IsDisposed, "Expected that a transient service is not disposed when the container is not disposed.");
            }

            // ensure the services are disposed by the container
            Assert.True(service.IsDisposed, "Expected that transient service is disposed when disposing container.");
        }
    }
}