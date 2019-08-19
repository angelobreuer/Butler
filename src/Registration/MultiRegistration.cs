namespace Butler.Registration
{
    using System.Collections.Generic;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     An <see cref="IServiceRegistration"/> containing multiple.
    /// </summary>
    public sealed class MultiRegistration : IServiceRegistration
    {
        private readonly List<IServiceRegistration> _registrations;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiRegistration"/> class.
        /// </summary>
        public MultiRegistration(IEnumerable<IServiceRegistration> registrations)
        {
            _registrations = new List<IServiceRegistration>(registrations);
        }

        /// <summary>
        ///     Gets the service registrations.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS
        public IReadOnlyList<IServiceRegistration> Registrations => _registrations;
#else // SUPPORTS_READONLY_COLLECTIONS
        public IEnumerable<IServiceRegistration> Registrations => _registrations;
#endif // !SUPPORTS_READONLY_COLLECTIONS

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime { get; }

        /// <summary>
        ///     Creates the services for the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">the context</param>
        /// <returns>the service enumerable</returns>
        public IEnumerable<object> Create(ServiceResolveContext context)
        {
            for (var index = 0; index < _registrations.Count; index++)
            {
                yield return context.Resolver.Resolve(_registrations[index], context);// TODO add support for scopes
            }
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        object IServiceRegistration.Create(ServiceResolveContext context)
        {
            // TODO Add support for scopes
            return context.Resolver.Resolve(_registrations[0], context);
        }
    }
}