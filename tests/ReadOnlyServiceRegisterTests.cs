namespace Butler.Tests
{
    using Butler.Register;
    using Butler.Tests.Dummy;
    using System;
    using Xunit;

    /// <summary>
    ///     Contains tests for <see cref="ReadOnlyServiceRegister"/>.
    /// </summary>
    public sealed class ReadOnlyServiceRegisterTests
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyServiceRegisterTests"/> class.
        /// </summary>
        public ReadOnlyServiceRegisterTests()
            => ServiceRegister = new ReadOnlyServiceRegister(new DummyServiceRegistration[0]);

        /// <summary>
        ///     Gets the service register being tested.
        /// </summary>
        public IServiceRegister ServiceRegister { get; }

        /// <summary>
        ///     Tests registering a dummy service registration.
        /// </summary>
        [Fact]
        public void TestRegister()
        {
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister);

            Assert.Throws<InvalidOperationException>(
                () => ServiceRegister.Register(new DummyServiceRegistration()));
        }
    }
}