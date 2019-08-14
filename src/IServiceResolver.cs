namespace Butler
{
    using System;

    /// <summary>
    ///     Interface for service resolvers.
    /// </summary>
    public interface IServiceResolver
#if SUPPORTS_SERVICE_PROVIDER
        : IServiceProvider
#endif
    {
        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        object Resolve(Type serviceType);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        TService Resolve<TService>();

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        Lazy<TService> ResolveLazy<TService>();
    }
}