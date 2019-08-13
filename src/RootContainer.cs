namespace Butler
{
    using Butler.Register;
    using Butler.Registration;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
    public class RootContainer : ServiceRegister, IEnumerable<KeyValuePair<Type, IServiceRegistration>>, IEnumerable, IServiceRegister, IServiceProvider
    {
        /// <summary>
        ///     Gets the service registration enumerator.
        /// </summary>
        /// <returns>the service registration enumerator</returns>
        public IEnumerator<KeyValuePair<Type, IServiceRegistration>> GetEnumerator()
            => Registrations.GetEnumerator();

        /// <summary>
        ///     Gets the service registration enumerator.
        /// </summary>
        /// <returns>the service registration enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type to resolve</param>
        /// <returns>
        ///     the resolved service; or <see langword="null"/> if the service could not been resolved
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        public object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            // TODO
            return null;
        }
    }
}