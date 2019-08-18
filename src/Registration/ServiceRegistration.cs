namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;
    using Butler.Util;

    /// <summary>
    ///     Basic service registration.
    /// </summary>
    /// <typeparam name="TImplementation">the type of the implementation</typeparam>
    public class ServiceRegistration<TImplementation> : BaseRegistration where TImplementation : class
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
#if NO_REFLECTION
        protected override object CreateService(ServiceResolveContext context) => Activator.CreateInstance<TImplementation>();
#else // NO_REFLECTION

        protected override object CreateService(ServiceResolveContext context) => Reflector.Resolve<TImplementation>(context);

#endif // !NO_REFLECTION
    }
}