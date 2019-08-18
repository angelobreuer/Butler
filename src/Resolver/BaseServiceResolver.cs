namespace Butler.Resolver
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Butler.Register;
    using Butler.Util;

    /// <summary>
    ///     Base implementation of an <see cref="IServiceResolver"/>.
    /// </summary>
    public abstract class BaseServiceResolver : ServiceRegister, IServiceResolver, IServiceRegister
    {
#if SUPPORTS_SERVICE_PROVIDER
        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        object IServiceProvider.GetService(Type serviceType) => Resolve(serviceType);
#endif

        /// <summary>
        ///     The default value for the <see cref="MaximumDepth"/> property.
        /// </summary>
        public const int DefaultMaximumDepth = 10;

        /// <summary>
        ///     The default value for the <see cref="ServiceConstructionMode"/> property.
        /// </summary>
        public const ServiceConstructionMode DefaultServiceConstructionMode = ServiceConstructionMode.Mixed;

        /// <summary>
        ///     The current <see cref="MaximumDepth"/>.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private int _maximumDepth = DefaultMaximumDepth;

        /// <summary>
        ///     The current <see cref="ServiceConstructionMode"/>.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private ServiceConstructionMode _serviceConstructionMode = DefaultServiceConstructionMode;

        /// <summary>
        ///     Gets or sets the maximum service resolve depth (used to detect self-referencing loops).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is zero or negative.
        /// </exception>
        public int MaximumDepth
        {
            get => _maximumDepth;

            set
            {
                // ensure the value is not zero or negative
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "The specified value can not be zero or negative!");
                }

                _maximumDepth = value;
            }
        }

        /// <summary>
        ///     Gets or sets the resolver's <see cref="ServiceConstructionMode"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the
        ///     <see cref="ServiceConstructionMode"/> enumeration.
        /// </exception>
        /// <exception cref="ArgumentException">thrown if the specified value is <see cref="ServiceConstructionMode.Default"/></exception>
        public ServiceConstructionMode ServiceConstructionMode
        {
            get => _serviceConstructionMode;

            set
            {
                // ensure the value is defined in the ServiceConstructionMode enumeration.
                if (!Enum.IsDefined(typeof(ServiceConstructionMode), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "The specified service construction mode is not defined.");
                }

                // ensure the value is valid for options
                if (value == ServiceConstructionMode.Default)
                {
                    throw new ArgumentException("The specified service construction mode 'Default' is not valid in this context.", nameof(value));
                }

                _serviceConstructionMode = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether disposable transients should be tracked by
        ///     the container. Note that enabling this property can cause memory leaks, because the
        ///     transients are released when the container is disposed.
        /// </summary>
        public bool TrackDisposableTransients { get; set; }

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
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
        public object Resolve(Type serviceType, object scopeKey = null, ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
        {
#if !NO_REFLECTION
#if SUPPORTS_REFLECTION
            // handle lazy resolving
            if (serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                // resolve service using parameters (e.g. Lazy<ISomeService> -> [ISomeService]
                serviceType = serviceType.GetGenericArguments()[0];

                return _resolveLazyMethod.MakeGenericMethod(serviceType)
                    .Invoke(this, new object[] { scopeKey, context, constructionMode });
            }
#else // SUPPORTS_REFLECTION
            var typeInformation = serviceType.GetTypeInfo();

            // handle lazy resolving
            if (typeInformation.IsGenericTypeDefinition && serviceType.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                // resolve service using parameters (e.g. Lazy<ISomeService> -> [ISomeService]
                serviceType = typeInformation.GenericTypeArguments[0];

                return _resolveLazyMethod.MakeGenericMethod(serviceType)
                    .Invoke(this, new object[] { scopeKey, context, constructionMode });
            }
#endif // !SUPPORTS_REFLECTION
#endif // !NO_REFLECTION

            // resolve service normally
            return ResolveService(serviceType, scopeKey, context, constructionMode);
        }

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <remarks>
        ///     This method is directly resolving services, the
        ///     <see cref="Resolve(Type, object, ServiceResolveContext, ServiceConstructionMode)"/>
        ///     handles the creation of lazy, function, etc. wrappers.
        /// </remarks>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
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
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        protected abstract object ResolveService(Type serviceType, object scopeKey = null, ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        public TService Resolve<TService>(object scopeKey = null, ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
            => (TService)Resolve(typeof(TService), scopeKey, context, constructionMode);

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        public Lazy<TService> ResolveLazy<TService>(object scopeKey = null, ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
            => new Lazy<TService>(() => Resolve<TService>(scopeKey, context, constructionMode));

#if !NO_REFLECTION
        /// <summary>
        ///     The method information of the
        ///     <see cref="ResolveLazy{TService}(object, ServiceResolveContext, ServiceConstructionMode)"/> method.
        /// </summary>
#if NET35
        private static readonly MethodInfo _resolveLazyMethod = typeof(IServiceResolver).GetMethod("ResolveLazy",
#else // NET35
        private static readonly MethodInfo _resolveLazyMethod = typeof(IServiceResolver).GetRuntimeMethod("ResolveLazy",
#endif // !NET35
            new[] { typeof(object), typeof(ServiceResolveContext), typeof(ServiceConstructionMode) });

#endif // !NO_REFLECTION
    }
}