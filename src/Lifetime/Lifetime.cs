namespace Butler.Lifetime
{
    using System;
    using Butler.Resolver;

    /// <summary>
    ///     Basic implementation of an <see cref="IServiceLifetime"/>.
    /// </summary>
    public sealed class Lifetime : IServiceLifetime
    {
        /// <summary>
        ///     Gets a singleton instance of the transient <see cref="IServiceLifetime"/>. This
        ///     <see cref="IServiceLifetime"/> creates a new instance of the service each request.
        /// </summary>
        public static IServiceLifetime Transient { get; }
            = new Lifetime("Transient", resolver => new TransientLifetimeManager());

        /// <summary>
        ///     Gets a singleton instance of the scoped <see cref="IServiceLifetime"/>. This
        ///     <see cref="IServiceLifetime"/> creates for each scope key a new instance.
        /// </summary>
        public static IServiceLifetime Scoped { get; }
            = new Lifetime("Scoped", resolver => new ScopedLifetimeManager());

        /// <summary>
        ///     Gets a singleton instance of the singleton <see cref="IServiceLifetime"/>. This
        ///     <see cref="IServiceLifetime"/> creates a service only once.
        /// </summary>
        public static IServiceLifetime Singleton { get; }
            = new Lifetime("Singleton", resolver => new SingletonLifetimeManager());

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lifetime"/> class.
        /// </summary>
        /// <param name="name">
        ///     the friendly, human-readable name of the service lifetime (e.g. <c>"Transient"</c>,
        ///     <c>"Singleton"</c>, etc)
        /// </param>
        /// <param name="managerFactory">
        ///     the <see cref="LifetimeManagerFactory"/> used to create
        ///     <see cref="ILifetimeManager"/> instances
        /// </param>
        /// <exception cref="ArgumentException">
        ///     thrown if the specified <paramref name="name"/> is <see langword="null"/>, empty or
        ///     only consists of whitespaces.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="managerFactory"/> is <see langword="null"/>.
        /// </exception>
        public Lifetime(string name, LifetimeManagerFactory managerFactory)
        {
#if SUPPORTS_WHITESPACE_CHECK
            if (string.IsNullOrWhiteSpace(name))
#else // SUPPORTS_WHITESPACE_CHECK
            if (string.IsNullOrEmpty(name))
#endif // !SUPPORTS_WHITESPACE_CHECK
            {
                throw new ArgumentException("The specified lifetime name can not be blank.", nameof(name));
            }

            Name = name;
            ManagerFactory = managerFactory ?? throw new ArgumentNullException(nameof(managerFactory));
        }

        /// <summary>
        ///     Gets the <see cref="LifetimeManagerFactory"/> used to create
        ///     <see cref="ILifetimeManager"/> instances.
        /// </summary>
        public LifetimeManagerFactory ManagerFactory { get; }

        /// <summary>
        ///     Gets the friendly, human-readable name of the service lifetime (e.g.
        ///     <c>"Transient"</c>, <c>"Singleton"</c>, etc).
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Creates a lifetime manager for the service lifetime.
        /// </summary>
        /// <param name="resolver">the calling resolver</param>
        /// <returns>the <see cref="ILifetimeManager"/> instance</returns>
        public ILifetimeManager CreateManager(IServiceResolver resolver) => ManagerFactory(resolver);
    }
}