namespace Butler.Lifetime
{
    using Butler.Resolver;

    /// <summary>
    ///     Interface for service lifetimes.
    /// </summary>
    public interface IServiceLifetime
    {
        /// <summary>
        ///     Gets the friendly, human-readable name of the service lifetime (e.g.
        ///     <c>"Transient"</c>, <c>"Singleton"</c>, etc).
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Creates a lifetime manager for the service lifetime.
        /// </summary>
        /// <param name="resolver">the calling resolver</param>
        /// <returns>the <see cref="ILifetimeManager"/> instance</returns>
        ILifetimeManager CreateManager(IServiceResolver resolver);
    }
}