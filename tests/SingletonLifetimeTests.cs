namespace Butler.Tests
{
    using Butler.Lifetime;
    using Butler.Tests.Dummy;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="Lifetime.Singleton"/> service lifetime.
    /// </summary>
    public sealed class SingletonLifetimeTests
    {
        /// <summary>
        ///     Tests that Singleton registrations return the same services when resolving them.
        /// </summary>
        [Fact]
        public void TestSingletonRegistration()
        {
            // create root container for test
            using (var container = new RootContainer())
            {
                // register the test service as Singleton
                container.Register<IDummyService, DummyService>().AsSingleton();

                // create two Singleton services (should be the same)
                var dummyService1 = container.Resolve<IDummyService>();
                var dummyService2 = container.Resolve<IDummyService>();

                // ensure the services are the same creations
                Assert.True(ReferenceEquals(dummyService1, dummyService2),
                    "Expected that Singleton services have the same reference.");
            }
        }

        /// <summary>
        ///     Tests that tracked Singleton services are disposed after disposing the
        ///     <see cref="RootContainer"/> in the Singleton service lifetime.
        /// </summary>
        [Fact]
        public void TestTrackingSingleton()
        {
            // create root container
            var container = new RootContainer();

            // register the dummy test service as Singleton
            container.Register<DummyDisposeTracker, DummyDisposeTracker>().AsSingleton();

            // resolve tests services
            var service = container.Resolve<DummyDisposeTracker>();

            // dispose container after usage
            using (container)
            {
                // service null checks
                Assert.NotNull(service);

                // ensure the services are not disposed if the container is not
                Assert.False(service.IsDisposed, "Expected that a Singleton service is not disposed when the container is not disposed.");
            }

            // ensure the services are disposed by the container
            Assert.True(service.IsDisposed, "Expected that Singleton service is disposed when disposing container.");
        }
    }
}