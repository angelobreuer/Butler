namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using Butler.Registration;

    /// <summary>
    ///     Interface for a service register.
    /// </summary>
    public interface IServiceRegister
    {
        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        ReadOnlyServiceRegister AsReadOnly();

        /// <summary>
        ///     Finds the service registration for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">the type of the service to find the registration for</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <returns>the service registration</returns>
        IServiceRegistration FindRegistration(Type type);

        /// <summary>
        ///     Finds the service registration for the specified <typeparamref name="TService" />.
        /// </summary>
        /// <typeparam name="TService">the type of the service to find the service for</typeparam>
        /// <returns>the service registration</returns>
        IServiceRegistration FindRegistration<TService>();

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="type">the type of the registration</param>
        /// <param name="registration">the registration to register</param>
        /// <param name="registrationMode">the service registration mode</param>
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
        ///     and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        void Register(Type type, IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the registration</typeparam>
        /// <param name="registration">the registration to register</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and replace is <see langword="false"/>.
        /// </exception>
        void Register<TService>(IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TService">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        void RegisterInstance<TService>(TService instance, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TService : class;

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TService">the type of the registration</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation</typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        void RegisterInstance<TService, TImplementation>(TImplementation instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : class, TService;
    }
}