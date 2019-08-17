namespace Butler.Resolver
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
        ///     Gets or sets the maximum service resolve depth (used to detect self-referencing loops).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is zero or negative.
        /// </exception>
        int MaximumDepth { get; set; }

        /// <summary>
        ///     Gets or sets the resolvers's <see cref="ServiceConstructionMode"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the
        ///     <see cref="ServiceConstructionMode"/> enumeration.
        /// </exception>
        /// <exception cref="ArgumentException">thrown if the specified value is <see cref="ServiceConstructionMode.Default"/></exception>
        ServiceConstructionMode ServiceConstructionMode { get; set; }

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        object Resolve(Type serviceType, ServiceResolveContext context = null, ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        TService Resolve<TService>(ServiceResolveContext context = null, ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        Lazy<TService> ResolveLazy<TService>(ServiceResolveContext context = null, ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);
    }
}