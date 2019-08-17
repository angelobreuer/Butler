namespace Butler.Lifetime
{
    using Butler.Resolver;

    /// <summary>
    ///     A delegate for <see cref="ILifetimeManager"/> factories.
    /// </summary>
    /// <param name="resolver">the calling resolver</param>
    /// <returns>the lifetime manager</returns>
    public delegate ILifetimeManager LifetimeManagerFactory(IServiceResolver resolver);
}