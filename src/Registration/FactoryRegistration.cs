﻿namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     Provides a factory registration.
    /// </summary>
    /// <typeparam name="TService">the service type</typeparam>
    public sealed class FactoryRegistration<TService> : IServiceRegistration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FactoryRegistration{TService}"/> class.
        /// </summary>
        /// <param name="factory">the service factory delegate</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public FactoryRegistration(ServiceFactory<TService> factory)
            => Factory = factory ?? throw new ArgumentNullException(nameof(factory));

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; } = Lifetime.Transient;

        /// <summary>
        ///     Gets the service factory.
        /// </summary>
        public ServiceFactory<TService> Factory { get; }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context) => Factory(context);
    }
}