namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Butler.Lifetime;
    using Butler.Registration;
    using Butler.Resolver;

#if SUPPORTS_LINQ
    using System.Linq;
#endif // SUPPORTS_LINQ

    /// <summary>
    ///     A class that manages service registrations.
    /// </summary>
    public class ServiceRegister : IServiceRegister
    {
        /// <summary>
        ///     The registrations keyed by the service type.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private readonly Dictionary<Type, IServiceRegistration> _registrations;

        /// <summary>
        ///     Lock used for accessing the <see cref="_registrations"/> thread-safe.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private readonly object _registrationsLock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegister"/> class.
        /// </summary>
        public ServiceRegister()
        {
            _registrations = new Dictionary<Type, IServiceRegistration>();
            _registrationsLock = new object();
        }

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///     Gets all copy of the service registrations in the register.
        /// </summary>

#if SUPPORTS_READONLY_COLLECTIONS

        public IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations
#else // SUPPORTS_READONLY_COLLECTIONS
        public IEnumerable<KeyValuePair<Type, IServiceRegistration>> Registrations
#endif // !SUPPORTS_READONLY_COLLECTIONS
        {
            get
            {
                lock (_registrationsLock)
                {
#if SUPPORTS_LINQ
                    return _registrations.ToArray();
#else // SUPPORTS_LINQ
                    // the index in the output array
                    var index = 0;

                    // the registrations array
                    var registrations = new KeyValuePair<Type, IServiceRegistration>[_registrations.Count];

                    // iterate through all keys
                    foreach (var key in _registrations.Keys)
                    {
                        // export registration to array
                        registrations[index++] = new KeyValuePair<Type, IServiceRegistration>(key, _registrations[key]);
                    }

                    return registrations;
#endif // !SUPPORTS_LINQ
                }
            }
        }

        /// <summary>
        ///     Gets or sets the default service lifetime when no specific lifetime was specified.
        /// </summary>
        public IServiceLifetime DefaultServiceLifetime { get; set; }
            = Lifetime.Transient;

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
        {
            // acquire lock
            lock (_registrationsLock)
            {
                // create register
                return new ReadOnlyServiceRegister(DefaultServiceLifetime,
                    new Dictionary<Type, IServiceRegistration>(_registrations));
            }
        }

        /// <summary>
        ///     Finds the service registration for the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">the type of the service to find the registration for</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <returns>the service registration</returns>
        public IServiceRegistration FindRegistration(Type type)
        {
            // null-check for type
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // acquire lock
            lock (_registrationsLock)
            {
                // try resolving the service registration
                return _registrations.TryGetValue(type, out var registration)
                    ? registration : default;
            }
        }

        /// <summary>
        ///     Finds the service registration for the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to find the service for</typeparam>
        /// <returns>the service registration</returns>
        public IServiceRegistration FindRegistration<TService>()
        {
            // acquire lock
            lock (_registrationsLock)
            {
                // try resolving the service registration
                return _registrations.TryGetValue(typeof(TService), out var registration)
                    ? registration : default;
            }
        }

        /// <summary>
        ///     Registers a service registration for the specified <typeparamref name="TService"/>.
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
        public ServiceRegistration<TImplementation> Register<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TImplementation : class, TService
        {
            var registration = new ServiceRegistration<TImplementation>();
            Register<TService>(registration.WithLifetime(lifetime ?? DefaultServiceLifetime), registrationMode);
            return registration;
        }

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
        public IServiceRegistration Register(Type type, IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            RegisterInternal(type, registration, registrationMode);
            return registration;
        }

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the registration</typeparam>
        /// <param name="registration">the registration to register</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <returns>the service registration added</returns>
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
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        public IServiceRegistration Register<TService>(IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
        {
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            RegisterInternal(typeof(TService), registration, registrationMode);
            return registration;
        }

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
        ///     exists and replace is <see langword="false"/>.
        /// </exception>
        public DirectRegistration<TImplementation> RegisterDirect<TService, TImplementation>(
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : TService, new()
        {
            var registration = new DirectRegistration<TImplementation>();
            Register<TService>(registration.WithLifetime(lifetime ?? DefaultServiceLifetime), registrationMode);
            return registration;
        }

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
        {
            var registration = new FactoryRegistration<TService>(factory);
            Register<TService>(registration.WithLifetime(lifetime ?? DefaultServiceLifetime), registrationMode);
            return registration;
        }

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
        {
            var registration = new FactoryRegistration<TImplementation>(factory);
            Register<TService>(registration.WithLifetime(lifetime ?? DefaultServiceLifetime), registrationMode);
            return registration;
        }

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
        public FactoryRegistration<TImplementation> RegisterFactory<TImplementation>(
            Type serviceType,
            ServiceFactory<TImplementation> factory,
            IServiceLifetime lifetime = null,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var registration = new FactoryRegistration<TImplementation>(factory);
            Register(serviceType, registration.WithLifetime(lifetime ?? DefaultServiceLifetime), registrationMode);
            return registration;
        }

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
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        public InstanceRegistration RegisterInstance<TService>(
            TService instance, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where TService : class
        {
            var registration = new InstanceRegistration(instance);
            Register<TService>(registration, registrationMode);
            return registration;
        }

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
            where TImplementation : class, TService
        {
            var registration = new InstanceRegistration(instance);
            Register<TService>(registration, registrationMode);
            return registration;
        }

        private void RegisterInternal(Type type, IServiceRegistration registration, ServiceRegistrationMode registrationMode)
        {
            // acquire lock
            lock (_registrationsLock)
            {
                // check if a service registration already exists for the type
                if (registrationMode != ServiceRegistrationMode.Replace && _registrations.TryGetValue(type, out var currentRegistration))
                {
                    // check if the exception should be suppressed
                    if (registrationMode == ServiceRegistrationMode.Ignore)
                    {
                        return;
                    }

                    // check if the registration should be appended
                    if (registrationMode == ServiceRegistrationMode.Append)
                    {
                        _registrations[type] = new MultiRegistration(new[] { currentRegistration, registration });
                        return;
                    }

                    // throw
                    throw new RegistrationException($"A service of type '{type}' has been already registered.");
                }

                // register registration
                _registrations[type] = registration;
            }
        }
    }
}