namespace Butler.Resolver
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Registration;
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
        ///     Resolves a service directly from the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to resolve the service from</param>
        /// <param name="context">the service resolver context</param>
        /// <param name="scopeKey">
        ///     the current scope key; or <see langword="null"/> to use the global scope
        /// </param>
        /// <returns>the resolved service</returns>
        object Resolve(IServiceRegistration registration, ServiceResolveContext context, object scopeKey = null);

        /// <summary>
        ///     Gets or sets the default <see cref="ServiceResolveMode"/> that is used when
        ///     <see cref="ServiceResolveMode.Default"/> is passed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the <see cref="ServiceResolveMode"/> enumeration.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is <see cref="ServiceResolveMode.Default"/> (this mode
        ///     can not be used, because the resolver specifies the default value, this mode can only
        ///     be used when passing to a resolver method)
        /// </exception>
        ServiceResolveMode DefaultResolveMode { get; set; }

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
        ///     Gets or sets a value indicating whether disposable transients should be tracked by
        ///     the container. Note that enabling this property can cause memory leaks, because the
        ///     transients are released when the container is disposed.
        /// </summary>
        bool TrackDisposableTransients { get; set; }

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="DefaultResolveMode"/> is used.
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
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        object Resolve(Type serviceType, object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        TService Resolve<TService>(object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        Lazy<TService> ResolveLazy<TService>(object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves multiple services of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>
        ///     an enumerable that enumerates through the services. The service enumerable caches the
        ///     service creations and creates the service as iterated (if not already cached).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        IEnumerable ResolveAll(Type serviceType, object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves multiple services of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the services to resolve</typeparam>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>
        ///     an enumerable that enumerates through the services. The service enumerable caches the
        ///     service creations and creates the service as iterated (if not already cached).
        /// </returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        IEnumerable<TService> ResolveAll<TService>(object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);
    }
}