namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Butler.Registration;

    /// <summary>
    ///     A class that manages service registrations.
    /// </summary>
    public class ServiceRegister : BaseServiceRegister, IServiceRegister
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
        public override bool IsReadOnly => false;

        /// <summary>
        ///     Gets all copy of the service registrations in the register.
        /// </summary>
        public override IReadOnlyList<KeyValuePair<Type, IServiceRegistration>> Registrations
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
        public override void Register(Type type, IServiceRegistration registration)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            _registrations.Add(type, registration);
        }
    }
}