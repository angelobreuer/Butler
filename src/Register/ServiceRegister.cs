namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using Butler.Registration;

    /// <summary>
    ///     A class that manages service registrations.
    /// </summary>
    public class ServiceRegister : IServiceRegister
    {
        private readonly List<IServiceRegistration> _registrations;
        private readonly object _registrationsLock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegister"/> class.
        /// </summary>
        public ServiceRegister()
        {
            _registrations = new List<IServiceRegistration>();
            _registrationsLock = new object();
        }

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly { get; } = false;

        /// <summary>
        ///     Gets all copy of the service registrations in the register.
        /// </summary>
        public IReadOnlyList<IServiceRegistration> Registrations
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
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        public void Register(IServiceRegistration registration)
        {
            // null-check
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            // acquire lock
            lock (_registrationsLock)
            {
                // add registration
                _registrations.Add(registration);
            }
        }
    }
}