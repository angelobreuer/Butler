namespace Butler.Registration
{
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     Interface for basic service registrations.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        object Create(ServiceResolveContext context);

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        IServiceLifetime ServiceLifetime { get; }
    }
}