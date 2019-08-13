namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Register;
    using Butler.Registration;

    /// <summary>
    ///     Interface for a root container.
    /// </summary>
    public interface IRootContainer : IEnumerable<KeyValuePair<Type, IServiceRegistration>>, IEnumerable, IServiceRegister,
#if SUPPORTS_SERVICE_PROVIDER
        IServiceProvider,
#endif // SUPPORTS_SERVICE_PROVIDER

#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else
        IDisposable
#endif // SUPPORTS_ASYNC_DISPOSABLE
    {
        /// <summary>
        ///     Resolves a single service.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the service instance of type <paramref name="serviceType"/></returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        object Resolve(Type serviceType);
    }
}