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
            Assert.Throws<ArgumentNullException>(() => ServiceRegister.Register(null, null));
            Assert.Throws<ArgumentNullException>(() => ServiceRegister.Register(GetType(), null));
            Assert.Throws<ArgumentNullException>(() => ServiceRegister.Register(null, new DummyServiceRegistration()));
        }

        /// <summary>
        ///     Tests registering a dummy service registration.
        /// </summary>
        [Fact]
        public void TestRegisterInstance()
        {
            // check before insert
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            // insert
            var registration = new DummyServiceRegistration();
            ServiceRegister.RegisterInstance<object>(registration);

            // check after insert
            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);

            // try resolve registration
            Assert.NotNull(ServiceRegister.FindRegistration<object>());
            Assert.Same(registration, ServiceRegister.FindRegistration<object>());
        }
    }
}