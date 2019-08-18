namespace Butler.Tests
{
    using Butler.Lifetime;
    using Butler.Tests.Dummy;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="Lifetime.Scoped"/> service lifetime.
    /// </summary>
    public sealed class ScopedLifetimeTests
    {
        /// <summary>
        ///     Tests that scoped registrations return the same / different results for same /
        ///     different scope keys.
        /// </summary>
        [Fact]
        public void TestScopedRegistration()
        {
            // create root container
            using (var container = new RootContainer())
            {
                // create dummy scope keys
                var globalScope = default(object);
                var myScope = new object();

                // register the test service as scoped
                container.Register<IDummyService, DummyService>().AsScoped();

                var dummyService1 = container.Resolve<IDummyService>(globalScope);
                var dummyService2 = container.Resolve<IDummyService>(globalScope);
                var dummyService3 = container.Resolve<IDummyService>(myScope);
                var dummyService4 = container.Resolve<IDummyService>(myScope);

                // ensure the created services are not null
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
        }

        /// <summary>
        ///     Tests that tracked scoped services are disposed after disposing the
        ///     <see cref="RootContainer"/> in the scoped service lifetime.
        /// </summary>
        [Fact]
        public void TestTrackingScoped()
        {
            // create root container
            var container = new RootContainer();

            // register the dummy test service as scoped
            container.Register<DummyDisposeTracker, DummyDisposeTracker>().AsScoped();

            // resolve tests services (they are different instances due they are resolved from
            // different scope keys)
            var dummyService1 = container.Resolve<DummyDisposeTracker>(scopeKey: null);
            var dummyService2 = container.Resolve<DummyDisposeTracker>(scopeKey: new object());

            // dispose container after usage
            using (container)
            {
                // ensure the services are different creations
                Assert.False(ReferenceEquals(dummyService1, dummyService2),
                "Expected that resolving using different scopes does not returns the same service.");

                // service null checks
                Assert.NotNull(dummyService1);
                Assert.NotNull(dummyService2);

                // ensure the services are not disposed if the container is not
                Assert.False(dummyService1.IsDisposed, "Expected that a scoped service is not disposed when the container is not disposed.");
                Assert.False(dummyService2.IsDisposed, "Expected that a scoped service is not disposed when the container is not disposed.");
            }

            // ensure the services are disposed by the container
            Assert.True(dummyService1.IsDisposed, "Expected that scoped service is disposed when disposing container (global scope).");
            Assert.True(dummyService2.IsDisposed, "Expected that scoped service is disposed when disposing container (non-global scope).");
        }
    }
}