﻿namespace Butler.Tests
{
    using System;
    using System.Collections.Generic;
    using Butler.Register;
    using Butler.Registration;
    using Butler.Tests.Dummy;
    using Butler.Lifetime;
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
            => ServiceRegister = new ReadOnlyServiceRegister(ServiceRegistrationMode.Throw,
                Lifetime.Singleton, new Dictionary<Type, IServiceRegistration>());

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
            Assert.Empty(ServiceRegister.Registrations);

            Assert.Throws<InvalidOperationException>(
                () => ServiceRegister.RegisterInstance(new DummyServiceRegistration()));
        }
    }
}