namespace Butler.Registration
{
    using Butler.Util;

    /// <summary>
    ///     Basic service registration.
    /// </summary>
    /// <typeparam name="TImplementation">the type of the implementation</typeparam>
    public class ServiceRegistration<TImplementation> : IServiceRegistration where TImplementation : class
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context) => Reflector.Resolve<TImplementation>(context);
    }
}