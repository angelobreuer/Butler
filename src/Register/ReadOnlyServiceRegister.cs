namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Butler.Lifetime;
    using Butler.Registration;
    using Butler.Resolver;

    /// <summary>
    ///     A class that provides a read-only service register.
    /// </summary>
    public sealed class ReadOnlyServiceRegister : IServiceRegister
    {
        private readonly IServiceLifetime _defaultServiceLifetime;

        /// <summary>
        ///     Gets or sets the default service lifetime when no specific lifetime was specified.
        /// </summary>
        public IServiceLifetime DefaultServiceLifetime
        {
            get => _defaultServiceLifetime;
            set => throw ThrowReadOnlyException();
        }

        /// <summary>
        ///     The service registrations.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS
        private readonly IReadOnlyDictionary<Type, IServiceRegistration> _registrations;
#else // SUPPORTS_READONLY_COLLECTIONS
        private readonly IDictionary<Type, IServiceRegistration> _registrations;
#endif // !SUPPORTS_READONLY_COLLECTIONS

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyServiceRegister"/> class.
        /// </summary>
        /// <param name="defaultServiceLifetime">the default service lifetime</param>
        /// <param name="registrations">the static registrations</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="defaultServiceLifetime"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registrations"/> dictionary is <see langword="null"/>.
        /// </exception>
#if SUPPORTS_READONLY_COLLECTIONS

        public ReadOnlyServiceRegister(IServiceLifetime defaultServiceLifetime, IReadOnlyDictionary<Type, IServiceRegistration> registrations)
#else // SUPPORTS_READONLY_COLLECTIONS

        public ReadOnlyServiceRegister(IServiceLifetime defaultServiceLifetime, IDictionary<Type, IServiceRegistration> registrations)

#endif // !SUPPORTS_READONLY_COLLECTIONS
        {
            _defaultServiceLifetime = defaultServiceLifetime ?? throw new ArgumentNullException(nameof(defaultServiceLifetime));
            _registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));
        }

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly { get; } = true;

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS
        public IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations => _registrations;
#else // SUPPORTS_READONLY_COLLECTIONS
        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> Registrations => _registrations;
#endif // !SUPPORTS_READONLY_COLLECTIONS

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
            => new ReadOnlyServiceRegister(DefaultServiceLifetime, _registrations);

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
        ///     Finds the service registration for the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to find the service for</typeparam>
        /// <returns>the service registration</returns>
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        public IServiceRegistration FindRegistration<TService>()
            => FindRegistration(typeof(TService));

        /// <summary>
        ///     Registers a service registration for the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation of the service</typeparam>
        /// <param name="registrationMode">the service registration mode</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="IServiceRegister.DefaultServiceLifetime"/> is used.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <typeparamref name="TService"/> already
        ///     exists and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        public ServiceRegistration<TImplementation> Register<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : class, TService => throw ThrowReadOnlyException();

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
        public IServiceRegistration Register(Type type, IServiceRegistration registration,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

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
        public IServiceRegistration Register<TService>(IServiceRegistration registration,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers a direct parameterless constructor service.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation of the service</typeparam>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="IServiceRegister.DefaultServiceLifetime"/> is used.
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
        public DirectRegistration<TImplementation> RegisterDirect<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : TService, new() => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TService">the service the factory provides</typeparam>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="IServiceRegister.DefaultServiceLifetime"/> is used.
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
        public FactoryRegistration<TService> RegisterFactory<TService>(ServiceFactory<TService> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers multiple services using registrations.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <param name="registrations">the service registrations</param>
        /// <param name="registrationMode">the registration mode</param>
        /// <returns>the multi-registration containing the registrations</returns>
        public MultiRegistration RegisterAll<TService>(IEnumerable<IServiceRegistration> registrations,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers multiple services using registrations.
        /// </summary>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="registrations">the service registrations</param>
        /// <param name="registrationMode">the registration mode</param>
        /// <returns>the multi-registration containing the registrations</returns>
        public MultiRegistration RegisterAll(Type serviceType, IEnumerable<IServiceRegistration> registrations,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TService">the service the factory provides</typeparam>
        /// <typeparam name="TImplementation">type of the service implementation</typeparam>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="IServiceRegister.DefaultServiceLifetime"/> is used.
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
        public FactoryRegistration<TImplementation> RegisterFactory<TService, TImplementation>(
            ServiceFactory<TImplementation> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

        /// <summary>
        ///     Registers a service factory.
        /// </summary>
        /// <typeparam name="TImplementation">type of the service implementation</typeparam>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="factory">the service factory</param>
        /// <param name="lifetime">
        ///     the service lifetime, if <see langword="null"/> the
        ///     <see cref="IServiceRegister.DefaultServiceLifetime"/> is used.
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
        public FactoryRegistration<TImplementation> RegisterFactory<TImplementation>(Type serviceType,
            ServiceFactory<TImplementation> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            => throw ThrowReadOnlyException();

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
        public InstanceRegistration RegisterInstance<TService>(TService instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TService : class => throw ThrowReadOnlyException();

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
        public InstanceRegistration RegisterInstance<TService, TImplementation>(TImplementation instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : class, TService => throw ThrowReadOnlyException();

        /// <summary>
        ///     Throws an exception that indicates that the service register is read-only.
        /// </summary>
        /// <exception cref="InvalidOperationException">always thrown</exception>
        private InvalidOperationException ThrowReadOnlyException()
            => throw new InvalidOperationException("The register is read-only. No registrations are allowed.");
    }
}