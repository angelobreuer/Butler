namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;
    using Butler.Util;
    using static Butler.Lifetime.Lifetime;

    /// <summary>
    ///     Basic service registration.
    /// </summary>
    /// <typeparam name="TImplementation">the type of the implementation</typeparam>
    public class ServiceRegistration<TImplementation> : IServiceRegistration where TImplementation : class
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegistration{TImplementation}"/> class.
        /// </summary>
        /// <param name="serviceLifetime">the initial service lifetime to use</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceLifetime"/> is <see langword="null"/>.
        /// </exception>
        public ServiceRegistration(IServiceLifetime serviceLifetime)
            => ServiceLifetime = serviceLifetime ?? throw new ArgumentNullException(nameof(serviceLifetime));

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; set; }

        /// <summary>
        ///     Registers the service with the <see cref="Scoped"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="ServiceRegistration{TImplementation}"/> instance</returns>
        public ServiceRegistration<TImplementation> AsScoped() => WithLifetime(Scoped);

        /// <summary>
        ///     Registers the service with the <see cref="Singleton"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="ServiceRegistration{TImplementation}"/> instance</returns>
        public ServiceRegistration<TImplementation> AsSingleton() => WithLifetime(Singleton);

        /// <summary>
        ///     Registers the service with the <see cref="Transient"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="ServiceRegistration{TImplementation}"/> instance</returns>
        public ServiceRegistration<TImplementation> AsTransient() => WithLifetime(Transient);

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
#if NO_REFLECTION
        public object Create(ServiceResolveContext context) => Activator.CreateInstance<TImplementation>();
#else // NO_REFLECTION

        public object Create(ServiceResolveContext context) => Reflector.Resolve<TImplementation>(context);

#endif // !NO_REFLECTION

        /// <summary>
        ///     Registers the service with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">the service lifetime</param>
        /// <returns>the <see cref="ServiceRegistration{TImplementation}"/> instance</returns>
        public ServiceRegistration<TImplementation> WithLifetime(IServiceLifetime lifetime)
        {
            ServiceLifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            return this;
        }
    }
}