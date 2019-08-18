namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using Butler.Lifetime;
    using Butler.Registration;
    using Butler.Resolver;

    /// <summary>
    ///     Interface for a service register.
    /// </summary>
    public interface IServiceRegister
    {
        /// <summary>
        ///     Gets or sets the default service lifetime when no specific lifetime was specified.
        /// </summary>
        IServiceLifetime DefaultServiceLifetime { get; set; }

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS
        IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }
#else // SUPPORTS_READONLY_COLLECTIONS
        IEnumerable<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }
#endif // !SUPPORTS_READONLY_COLLECTIONS

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
        ///     Finds the service registration for the specified <typeparamref name="TService"/>.
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
        IServiceRegistration Register(Type type, IServiceRegistration registration,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

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
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        IServiceRegistration Register<TService>(IServiceRegistration registration,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

        /// <summary>
        ///     Registers a service registration for the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation of the service</typeparam>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="DefaultServiceLifetime"/> is used.
        /// </param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        ServiceRegistration<TImplementation> Register<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TImplementation : class, TService;

        /// <summary>
        ///     Registers a direct parameterless constructor service.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation of the service</typeparam>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="DefaultServiceLifetime"/> is used.
        /// </param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        DirectRegistration<TImplementation> RegisterDirect<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TImplementation : TService, new();

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TService">the service the factory provides</typeparam>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="DefaultServiceLifetime"/> is used.
        /// </param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        FactoryRegistration<TService> RegisterFactory<TService>(ServiceFactory<TService> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TService">the service the factory provides</typeparam>
        /// <typeparam name="TImplementation">type of the service implementation</typeparam>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="DefaultServiceLifetime"/> is used.
        /// </param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        FactoryRegistration<TImplementation> RegisterFactory<TService, TImplementation>(
            ServiceFactory<TImplementation> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TImplementation">type of the service implementation</typeparam>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="DefaultServiceLifetime"/> is used.
        /// </param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="serviceType"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        FactoryRegistration<TImplementation> RegisterFactory<TImplementation>(
            Type serviceType, ServiceFactory<TImplementation> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default);

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
        InstanceRegistration RegisterInstance<TService>(TService instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TService : class;

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
        InstanceRegistration RegisterInstance<TService, TImplementation>(TImplementation instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : class, TService;
    }
}