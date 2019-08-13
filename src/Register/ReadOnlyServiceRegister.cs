﻿namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using Butler.Registration;

    /// <summary>
    ///     A class that provides a read-only service register.
    /// </summary>
    public sealed class ReadOnlyServiceRegister : IServiceRegister
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyServiceRegister"/> class.
        /// </summary>
        /// <param name="registrations">the static registrations</param>
        public ReadOnlyServiceRegister(IReadOnlyList<KeyValuePair<Type, IServiceRegistration>> registrations)
            => Registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly { get; } = true;

        /// <summary>
        ///     Gets all copy of the service registrations in the register.
        /// </summary>
        public IReadOnlyList<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
            => new ReadOnlyServiceRegister(Registrations);

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="type">the type of the registration</param>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register(Type type, IServiceRegistration registration)
            => ThrowReadOnlyException();

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register<T>(IServiceRegistration registration)
            => ThrowReadOnlyException();

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register<T>(T instance) where T : class
            => ThrowReadOnlyException();

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TAbstraction">the type of the registration</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register<TAbstraction, TImplementation>(TImplementation instance)
            where TImplementation : class, TAbstraction => ThrowReadOnlyException();

        /// <summary>
        ///     Throws an exception that indicates that the service register is read-only.
        /// </summary>
        /// <exception cref="InvalidOperationException">always thrown</exception>
        private void ThrowReadOnlyException()
            => throw new InvalidOperationException("The register is read-only. No registrations are allowed.");
    }
}