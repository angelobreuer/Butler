namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     Provides an <see cref="IServiceRegistration"/> for direct parameterless constructors.
    /// </summary>
    /// <typeparam name="TImplementation">the service implementation</typeparam>
    public class DirectRegistration<TImplementation>
        : BaseRegistration where TImplementation : new()
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        protected override object CreateService(ServiceResolveContext context) => new TImplementation();
    }
}