namespace Butler
{
    using Butler.Register;
    using Butler.Registration;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
    public class RootContainer : ServiceRegister, IRootContainer, IServiceResolver
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

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        public TService Resolve<TService>()
            => (TService)Resolve(typeof(TService));

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        public Lazy<TService> ResolveLazy<TService>()
            => new Lazy<TService>(() => Resolve<TService>());

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