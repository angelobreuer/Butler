namespace Butler.Registration
{
    using Butler.Resolver;

    /// <summary>
    ///     Provides an <see cref="IServiceRegistration"/> for direct parameterless constructors.
    /// </summary>
    /// <typeparam name="TImplementation">the service implementation</typeparam>
    public class DirectRegistration<TImplementation>
        : IServiceRegistration where TImplementation : new()
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context) => new TImplementation();
    }
}