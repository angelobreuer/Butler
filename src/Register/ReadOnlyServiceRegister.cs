namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Butler.Registration;

    /// <summary>
    ///     A class that provides a read-only service register.
    /// </summary>
    public sealed class ReadOnlyServiceRegister : IServiceRegister
    {
        /// <summary>
        ///     The service registrations.
        /// </summary>
        private readonly IReadOnlyDictionary<Type, IServiceRegistration> _registrations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyServiceRegister"/> class.
        /// </summary>
        /// <param name="registrations">the static registrations</param>
        public ReadOnlyServiceRegister(IReadOnlyDictionary<Type, IServiceRegistration> registrations)
            => _registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly { get; } = true;

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        public IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations => _registrations;

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
            => new ReadOnlyServiceRegister(_registrations);

        /// <summary>
        ///     Finds the service registration for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">the type of the service to find the registration for</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <returns>the service registration</returns>
        public IServiceRegistration FindRegistration(Type type)
            => _registrations.TryGetValue(type, out var registration) ? registration : null;

        /// <summary>
        ///     Finds the service registration for the specified <paramref name="type"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to find the service for</typeparam>
        /// <returns>the service registration</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IServiceRegistration FindRegistration<TService>()
            => FindRegistration(typeof(TService));

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
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and replace is <see langword="false"/>.
        /// </exception>
        public void Register(Type type, IServiceRegistration registration, bool replace = false)
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
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and replace is <see langword="false"/>.
        /// </exception>
        public void Register<T>(IServiceRegistration registration, bool replace = false)
            => ThrowReadOnlyException();

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and replace is <see langword="false"/>.
        /// </exception>
        public void RegisterInstance<T>(T instance, bool replace = false) where T : class
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
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and replace is <see langword="false"/>.
        /// </exception>
        public void RegisterInstance<TAbstraction, TImplementation>(TImplementation instance, bool replace = false)
            where TImplementation : class, TAbstraction
            => ThrowReadOnlyException();

        /// <summary>
        ///     Throws an exception that indicates that the service register is read-only.
        /// </summary>
        /// <exception cref="InvalidOperationException">always thrown</exception>
        private void ThrowReadOnlyException()
            => throw new InvalidOperationException("The register is read-only. No registrations are allowed.");
    }
}