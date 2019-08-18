namespace Butler.Tests
{
    using Butler.Tests.Dummy;
    using Xunit;

    /// <summary>
    ///     Contains tests for the <see cref="RootContainer"/> class targeting on child containers.
    /// </summary>
    public sealed class ChildContainerTests
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChildContainerTests"/> class.
        /// </summary>
        public ChildContainerTests()
        {
            RootContainer = new RootContainer();
            ChildContainer = RootContainer.CreateChild();
        }

        /// <summary>
        ///     Gets the root container.
        /// </summary>
        public IRootContainer RootContainer { get; }

        /// <summary>
        ///     Gets root container which is the child of the <see cref="RootContainer"/>.
        /// </summary>
        public IRootContainer ChildContainer { get; }

        /// <summary>
        ///     Tests the creation of child container.
        /// </summary>
        [Fact]
        public void TestChildContainerCreation()
        {
            Assert.NotNull(ChildContainer);
            Assert.NotNull(ChildContainer.Parent);
            Assert.Equal(RootContainer, ChildContainer.Parent);
        }

        [Fact]
        public void TestChildContainerParentResolveWithOverrideFromChild()
        {
            // allow service overriding from the child container
            ChildContainer.ContainerResolveMode = ContainerResolveMode.ChildFirst;

            // register service in parent container
            RootContainer.Register<IDummyService, DummyService>();

            // register service in child container
            ChildContainer.Register<IDummyService, OtherDummyService>();

            // try resolving the service in the parent container from the child container
            Assert.IsType<OtherDummyService>(ChildContainer.Resolve<IDummyService>());
        }

        [Fact]
        public void TestChildContainerParentResolveWithOverrideFromParent()
        {
            // allow service overriding from the child container
            ChildContainer.ContainerResolveMode = ContainerResolveMode.ParentFirst;

            // register service in parent container
            RootContainer.Register<IDummyService, DummyService>();

            // register service in child container
            ChildContainer.Register<IDummyService, OtherDummyService>();

            // try resolving the service in the parent container from the child container
            Assert.IsType<DummyService>(ChildContainer.Resolve<IDummyService>());
        }

        [Fact]
        public void TestChildContainerParentResolve()
        {
            // register service in parent container
            RootContainer.Register<IDummyService, DummyService>();

            // try resolving the service in the parent container from the child container
            Assert.NotNull(ChildContainer.Resolve<IDummyService>());
        }
    }
}