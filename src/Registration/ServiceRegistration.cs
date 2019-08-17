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
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context) => Reflector.Resolve<TImplementation>(context);

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; set; }

        public ServiceRegistration<TImplementation> WithLifetime(IServiceLifetime lifetime)
        {
            ServiceLifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            return this;
        }

        public ServiceRegistration<TImplementation> AsTransient() => WithLifetime(Transient);
    }
}