namespace Butler.Registration
{
    using System;

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
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Factory = factory;
        }

        /// <summary>
        ///     Gets the service factory.
        /// </summary>
        public ServiceFactory<TService> Factory { get; }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="resolver">the calling resolver</param>
        /// <returns>the instance</returns>
        public object Create(IServiceResolver resolver) => Factory(resolver);
    }
}