namespace Butler.Registration
{
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
    }
}