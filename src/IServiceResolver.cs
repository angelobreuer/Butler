namespace Butler
{
    using System;
    using Butler.Util;

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
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        object Resolve(Type serviceType, ServiceResolveContext context = null);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        TService Resolve<TService>(ServiceResolveContext context = null);

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        Lazy<TService> ResolveLazy<TService>(ServiceResolveContext context = null);
    }
}