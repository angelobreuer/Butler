namespace Butler.Registration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     An <see cref="IServiceRegistration"/> containing multiple registrations.
    /// </summary>
    public sealed class MultiRegistration : IServiceRegistration, IList<IServiceRegistration>
    {
        private const string CannotBeEmpty = "A multi-registration must contain at least a single registration";
        private const string FoundInconsistence = "Found service lifetime inconsistence. A multi-registration can not have registrations with different service lifetimes.";
        private readonly List<IServiceRegistration> _registrations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRegistration"/> class.
        /// </summary>
        /// <param name="registrations">the service registrations contained in the registration</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registrations"/> enumerable is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     thrown if the specified <paramref name="registrations"/> enumerable is empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetimes of the registrations in the specified
        ///     <paramref name="registrations"/> enumerable are inconsistent.
        /// </exception>
        public MultiRegistration(IEnumerable<IServiceRegistration> registrations)
        {
            if (registrations is null)
            {
                throw new ArgumentNullException(nameof(registrations));
            }

            _registrations = new List<IServiceRegistration>(registrations);

            // ensure there is at least one registration
            if (_registrations.Count == 0)
            {
                throw new ArgumentException(CannotBeEmpty, nameof(registrations));
            }

            // set the service lifetime
            ServiceLifetime = _registrations[0].ServiceLifetime;

            // iterate through all registrations (except the first, the first registration sets the
            // base lifetime)
            for (var index = 1; index < _registrations.Count; index++)
            {
                // ensure that the lifetime matches the first
                if (_registrations[index].ServiceLifetime != ServiceLifetime)
                {
                    // throw
                    throw new InvalidOperationException(FoundInconsistence);
                }
            }
        }

        /// <summary>
        ///     Gets the number of registrations.
        /// </summary>
        public int Count => _registrations.Count;

        /// <summary>
        ///     Gets a value indicating whether no registrations can be removed or added to the registration.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///     Gets the service registrations.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS
        public IReadOnlyList<IServiceRegistration> Registrations => _registrations;
#else // SUPPORTS_READONLY_COLLECTIONS
        public IEnumerable<IServiceRegistration> Registrations => _registrations;
#endif // !SUPPORTS_READONLY_COLLECTIONS

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; }

        /// <summary>
        ///     Gets or sets the <see cref="IServiceRegistration"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">the zero-based index</param>
        /// <returns>the service registration at the specified <paramref name="index"/></returns>
        /// <exception cref="ArgumentNullException">thrown if the specified value is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetime ( <see cref="IServiceRegistration.ServiceLifetime"/>)
        ///     does not match the registration lifetime ( <see cref="ServiceLifetime"/>).
        /// </exception>
        public IServiceRegistration this[int index]
        {
            get => _registrations[index];

            set
            {
                CheckLifetime(value);
                _registrations[index] = value;
            }
        }

        /// <summary>
        ///     Adds the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to add</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetime ( <see cref="IServiceRegistration.ServiceLifetime"/>)
        ///     does not match the registration lifetime ( <see cref="ServiceLifetime"/>).
        /// </exception>
        public void Add(IServiceRegistration registration)
        {
            CheckLifetime(registration);
            _registrations.Add(registration);
        }

        /// <summary>
        ///     Adds the specified <paramref name="registrations"/>.
        /// </summary>
        /// <param name="registrations">the registrations to add</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registrations"/> enumerable is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the one of the registrations in the specified
        ///     <paramref name="registrations"/> enumerable is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetime ( <see cref="IServiceRegistration.ServiceLifetime"/>)
        ///     of one of the service registrations in the specified <paramref name="registrations"/>
        ///     enumerable does not match the registration lifetime ( <see cref="ServiceLifetime"/>).
        /// </exception>
        public void AddRange(IEnumerable<IServiceRegistration> registrations)
        {
            if (registrations is null)
            {
                throw new ArgumentNullException(nameof(registrations));
            }

            foreach (var registration in registrations)
            {
                CheckLifetime(registration);
            }

            _registrations.AddRange(registrations);
        }

        /// <summary>
        ///     Clears all registrations.
        /// </summary>
        /// <exception cref="NotSupportedException">
        ///     thrown because all multi-registrations must contain at least a single registration.
        /// </exception>
        public void Clear() => throw new NotSupportedException(CannotBeEmpty);

        /// <summary>
        ///     Checks whether the service registration contains the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the service registration</param>
        /// <returns>
        ///     a value indicating whether the service registration contains the specified <paramref name="registration"/>
        /// </returns>
        public bool Contains(IServiceRegistration registration)
            => (registration is null || !registration.ServiceLifetime.Equals(ServiceLifetime)) && _registrations.Contains(registration);

        /// <summary>
        ///     Copies all registrations to the specified <paramref name="array"/> starting at the
        ///     specified zero-based <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">the array to copy the registrations to</param>
        /// <param name="arrayIndex">the write index</param>
        public void CopyTo(IServiceRegistration[] array, int arrayIndex) => _registrations.CopyTo(array, arrayIndex);

        /// <summary>
        ///     Creates the services for the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">the context</param>
        /// <returns>the service enumerable</returns>
        public IEnumerable<object> Create(ServiceResolveContext context)
        {
            for (var index = 0; index < _registrations.Count; index++)
            {
                var registration = _registrations[index];

                // ensure the lifetime is consistent
                CheckLifetime(registration);

                // resolve service
                yield return context.Resolver.Resolve(registration, context);
            }
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        object IServiceRegistration.Create(ServiceResolveContext context)
        {
            // TODO Add support for scopes
            return context.Resolver.Resolve(_registrations[0], context);
        }

        /// <summary>
        ///     Gets the registration enumerator.
        /// </summary>
        /// <returns>the registration enumerator</returns>
        public IEnumerator<IServiceRegistration> GetEnumerator() => ((IList<IServiceRegistration>)_registrations).GetEnumerator();

        /// <summary>
        ///     Gets the registration enumerator.
        /// </summary>
        /// <returns>the registration enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IList<IServiceRegistration>)_registrations).GetEnumerator();

        /// <summary>
        ///     Gets the zero-based index of the specified
        /// </summary>
        /// <param name="registration">the registration to find</param>
        /// <returns>
        ///     the absolute zero-based index of the specified <paramref name="registration"/>; if
        ///     negative then the specified <paramref name="registration"/> is not contained in the
        ///     multi-registration or the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </returns>
        public int IndexOf(IServiceRegistration registration)
            => registration is null || !registration.ServiceLifetime.Equals(ServiceLifetime)
                ? -1 : _registrations.IndexOf(registration);

        /// <summary>
        ///     Inserts the specified <paramref name="registration"/> at the specified zero-based <paramref name="index"/>.
        /// </summary>
        /// <param name="index">the zero-based index to insert the registration at</param>
        /// <param name="registration">the registration to insert</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetime ( <see cref="IServiceRegistration.ServiceLifetime"/>)
        ///     does not match the registration lifetime ( <see cref="ServiceLifetime"/>).
        /// </exception>
        public void Insert(int index, IServiceRegistration registration)
        {
            CheckLifetime(registration);

            _registrations.Insert(index, registration);
        }

        /// <summary>
        ///     Removes the specified <paramref name="registration"/> from the registration list.
        /// </summary>
        /// <param name="registration">the registration to remove</param>
        /// <returns>
        ///     a value indicating whether the specified <paramref name="registration"/> was
        ///     contained in the registration list and whether it was removed.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the registration list would be empty if the item is removed
        /// </exception>
        public bool Remove(IServiceRegistration registration)
        {
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (_registrations.Count - 1 <= 0)
            {
                throw new InvalidOperationException(CannotBeEmpty);
            }

            return _registrations.Remove(registration);
        }

        /// <summary>
        ///     Removes the registration at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">the zero-based index</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the registration list would be empty if the item is removed
        /// </exception>
        public void RemoveAt(int index)
        {
            if (Count - 1 <= 0)
            {
                throw new InvalidOperationException(CannotBeEmpty);
            }

            _registrations.RemoveAt(index);
        }

        /// <summary>
        ///     Checks for service lifetime inconsistence of the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the service registration</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the service lifetime ( <see cref="IServiceRegistration.ServiceLifetime"/>)
        ///     does not match the registration lifetime ( <see cref="ServiceLifetime"/>).
        /// </exception>
        private void CheckLifetime(IServiceRegistration registration)
        {
            if (registration is null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            if (!registration.ServiceLifetime.Equals(ServiceLifetime))
            {
                throw new InvalidOperationException(FoundInconsistence +
                    $"\nParent Lifetime: {ServiceLifetime.Name}, Child Lifetime: {registration.ServiceLifetime.Name}, Registration: {registration}.");
            }
        }
    }
}