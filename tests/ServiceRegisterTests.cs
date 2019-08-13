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
        ///     Tests registering a dummy service registration.
        /// </summary>
        [Fact]
        public void TestRegister()
        {
            // check before insert
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            // insert
            var registration = new DummyServiceRegistration();
            ServiceRegister.Register<object>(registration);

            // check after insert
            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);

            // try resolve registration
            Assert.NotNull(ServiceRegister.FindRegistration<object>());
            Assert.Same(registration, ServiceRegister.FindRegistration<object>());
        }

        /// <summary>
        ///     Tests registering a duplicate registration without setting the replace flag.
        /// </summary>
        [Fact]
        public void TestRegisterDuplicateInstance()
        {
            // check before insert
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            // insert
            var instance = new DummyDerivedClass();
            ServiceRegister.RegisterInstance<DummyBaseClass>(instance);

            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);

            // insert other (should throw exception)
            Assert.Throws<RegistrationException>(
                () => ServiceRegister.RegisterInstance<DummyBaseClass>(instance));
        }

        /// <summary>
        ///     Tries to register an instance.
        /// </summary>
        [Fact]
        public void TestRegisterInstance()
        {
            // check before insert
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            // insert
            var instance = new DummyDerivedClass();
            ServiceRegister.RegisterInstance<DummyBaseClass>(instance);

            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);

            // try resolve registration
            var registration = ServiceRegister.FindRegistration<DummyBaseClass>();
            Assert.NotNull(registration);
            Assert.Same(instance, registration.Create());
        }

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
        ///     Tests registering a <see langword="null"/> instance.
        /// </summary>
        [Fact]
        public void TestRegisterNullInstance()
            => Assert.Throws<ArgumentNullException>(() => ServiceRegister.RegisterInstance<object>(null));

        /// <summary>
        ///     Tests registering a duplicate registration with setting the replace flag.
        /// </summary>
        [Fact]
        public void TestRegisterReplace()
        {
            // check before insert
            Assert.NotNull(ServiceRegister);
            Assert.Empty(ServiceRegister.Registrations);

            // insert
            var instance = new DummyDerivedClass();
            ServiceRegister.RegisterInstance<DummyBaseClass>(instance);

            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);

            // try resolve registration
            var registration = ServiceRegister.FindRegistration<DummyBaseClass>();
            Assert.NotNull(registration);
            Assert.Same(instance, registration.Create());

            // replace registration
            var otherInstance = new DummyOtherDerivedClass();
            ServiceRegister.RegisterInstance<DummyBaseClass>(otherInstance, replace: true);

            // try resolve other registration
            var otherRegistration = ServiceRegister.FindRegistration<DummyBaseClass>();
            Assert.NotEmpty(ServiceRegister.Registrations);
            Assert.Single(ServiceRegister.Registrations);
            Assert.NotNull(otherRegistration);
            Assert.Same(otherInstance, otherRegistration.Create());
        }
    }
}