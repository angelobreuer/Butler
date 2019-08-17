namespace Butler.Resolver
{
    using System;
    using System.Diagnostics;
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
        public abstract object Resolve(Type serviceType, ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
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
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TService Resolve<TService>(ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
            => (TService)Resolve(typeof(TService), context);

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
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
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        public Lazy<TService> ResolveLazy<TService>(ServiceResolveContext context = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
            => new Lazy<TService>(() => Resolve<TService>(context));
    }
}