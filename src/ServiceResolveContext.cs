namespace Butler
{
    using System;
    using Butler.Util;

    /// <summary>
    ///     A compound of required information while resolving a service.
    /// </summary>
    public sealed class ServiceResolveContext
    {
#if DEBUG

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceResolveContext"/> class.
        /// </summary>
        /// <param name="resolver">the service resolver using the context</param>
        /// <param name="serviceType">the type of the service being resolved</param>
        /// <param name="parentType">the type of the parent</param>
        /// <param name="traceBuilder">
        ///     the trace builder; if <see langword="null"/> a new instance will be created.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="resolver"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        public ServiceResolveContext(IServiceResolver resolver, Type serviceType, Type parentType = null, TraceBuilder traceBuilder = null)
        {
            ParentType = parentType;
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            TraceBuilder = traceBuilder ?? new TraceBuilder();
        }

#else // DEBUG
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceResolveContext"/> class.
        /// </summary>
        /// <param name="resolver">the service resolver using the context</param>
        /// <param name="serviceType">the type of the service being resolved</param>
        /// <param name="parentType">the type of the parent</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="resolver"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        public ServiceResolveContext(IServiceResolver resolver, Type serviceType, Type parentType = null)
        {
            ParentType = parentType;
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }
#endif // !DEBUG

        /// <summary>
        ///     Gets the actual resolver depth. If a specific depth is reached an exception is thrown.
        /// </summary>
        public int Depth { get; internal set; }

        /// <summary>
        ///     Gets the type of the parent being resolved (if not <see langword="null"/>, then the
        ///     <see cref="ServiceType"/> is required to resolved as a dependency for the
        ///     <see cref="ParentType"/> service, <see langword="null"/> if the service is being
        ///     resolved directly).
        /// </summary>
        public Type ParentType { get; }

        /// <summary>
        ///     Gets the resolver that was used to resolve the service.
        /// </summary>
        public IServiceResolver Resolver { get; }

        /// <summary>
        ///     Gets the type of the service being service.
        /// </summary>
        public Type ServiceType { get; }

#if DEBUG

        /// <summary>
        ///     Gets the trace builder used for useful output on resolve failures to make it easier
        ///     to find resolution errors and their cause.
        /// </summary>
        /// <remarks>
        ///     Please note that this property is only available when the Debug configuration is used.
        /// </remarks>
        public TraceBuilder TraceBuilder { get; }

#endif // DEBUG
    }
}