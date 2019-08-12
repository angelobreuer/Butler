namespace Butler.Tests
{
    using Butler.Register;
    using Butler.Registration;
    using Butler.Tests.Dummy;
    using System;
    using Xunit;

    /// <summary>
    ///     Contains tests for <see cref="ServiceRegister"/>.
    /// </summary>
    public sealed class ServiceRegisterTests
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegisterTests"/> class.
        /// </summary>
        public ServiceRegisterTests()
            => ServiceRegister = new ServiceRegister();

        /// <summary>
        ///     Gets the service register being tested.
        /// </summary>
        public IServiceRegister ServiceRegister { get; }

        /// <summary>
        ///     Tests registering a <see langword="null"/><see cref="IServiceRegistration"/>.
        /// </summary>
        [Fact]
        public void TestRegisterNull()
        {
            Assert.Throws<ArgumentNullException>(() => ServiceRegister.Register(null));
        }

        /// <summary>
        ///     Tests registering a dummy service registration.
        /// </summary>
        [Fact]
        public void TestRegister()
        {
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            ServiceRegister.Register(new DummyServiceRegistration());

            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);
        }
    }
}