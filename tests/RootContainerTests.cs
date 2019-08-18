namespace Butler.Tests
{
    using System;
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
    }
}