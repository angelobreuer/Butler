namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Butler.Registration;

    /// <summary>
    ///     A class that manages service registrations.
    /// </summary>
    public class ServiceRegister : IServiceRegister
    {
        private readonly Dictionary<Type, IServiceRegistration> _registrations;
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
        public IReadOnlyCollection<KeyValuePair<Type, IServiceRegistration>> Registrations
        {
            get
            {
                lock (_registrationsLock)
                {
                    return _registrations.ToArray();
                }
            }
        }

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
            => new ReadOnlyServiceRegister(Registrations.ToDictionary(s => s.Key, s => s.Value));

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
        ///     Finds the service registration for the specified <paramref name="type"/>.
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
        public void Register(Type type, IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
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
        }

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="registration">the registration to register</param>
        /// <param name="registrationMode">the service registration mode</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Register<T>(IServiceRegistration registration, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
        {
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            RegisterInternal(typeof(T), registration, registrationMode);
        }

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RegisterInstance<T>(T instance, ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default) where T : class
            => Register<T>(new InstanceRegistration(instance), registrationMode);

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TAbstraction">the type of the registration</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation</typeparam>
        /// <param name="instance">the instance</param>
        /// <param name="registrationMode">the service registration mode</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        /// <exception cref="RegistrationException">
        ///     thrown if a registration with the specified <paramref name="type"/> already exists
        ///     and the specified <paramref name="registrationMode"/> is not
        ///     <see cref="ServiceRegistrationMode.Replace"/> or <see cref="ServiceRegistrationMode.Ignore"/>.
        /// </exception>
        public void RegisterInstance<TAbstraction, TImplementation>(TImplementation instance,
            ServiceRegistrationMode registrationMode = ServiceRegistrationMode.Default)
            where TImplementation : class, TAbstraction
            => Register<TAbstraction>(new InstanceRegistration(instance), registrationMode);

        private void RegisterInternal(Type type, IServiceRegistration registration, ServiceRegistrationMode registrationMode)
        {
            // acquire lock
            lock (_registrationsLock)
            {
                // check if a service registration already exists for the type
                if (registrationMode != ServiceRegistrationMode.Replace && _registrations.Remove(type))
                {
                    // check if the exception should be suppressed
                    if (registrationMode == ServiceRegistrationMode.Ignore)
                    {
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