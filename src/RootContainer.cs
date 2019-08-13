namespace Butler
{
    using Butler.Register;
    using Butler.Registration;
    using System;
    using System.Collections;
    using System.Collections.Generic;

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
    public class RootContainer : ServiceRegister, IRootContainer
    {
#if !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Disposes the root container.
        /// </summary>
        public void Dispose()
        {
        }

#endif // !SUPPORTS_ASYNC_DISPOSABLE

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

#if SUPPORTS_SERVICE_PROVIDER
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
        object IServiceProvider.GetService(Type serviceType) => Resolve(serviceType);
#endif // SUPPORTS_SERVICE_PROVIDER

        /// <summary>
        ///     Resolves a single service.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the service instance of type <paramref name="serviceType"/></returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        public object Resolve(Type serviceType)
        {
            // find registration
            var registration = FindRegistration(serviceType);

            // TODO TODO TODO
            return registration.Create();
        }

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes the root container asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation.</returns>
        public ValueTask DisposeAsync()
        {
            return default;
        }
#endif // SUPPORTS_ASYNC_DISPOSABLE
    }
}