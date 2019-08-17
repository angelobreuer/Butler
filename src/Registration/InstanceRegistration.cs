namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     Registration of a static instance.
    /// </summary>
    public sealed class InstanceRegistration : IServiceRegistration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InstanceRegistration"/> class.
        /// </summary>
        /// <param name="instance">the static instance</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public InstanceRegistration(object instance)
            => Instance = instance ?? throw new ArgumentNullException(nameof(instance));

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; } = Lifetime.Transient;

        /// <summary>
        ///     Gets the static instance.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context) => Instance;
    }
}