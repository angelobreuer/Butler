namespace Butler.Resolver
{
    using Butler.Util;

    /// <summary>
    ///     A set of different service resolution modes.
    /// </summary>
    public enum ServiceResolveMode
    {
        /// <summary>
        ///     Denotes that the default behavior of the <see cref="IServiceResolver"/> should be used.
        /// </summary>
        Default,

        /// <summary>
        ///     Denotes that the resolver should throw a <see cref="ResolverException"/> if the
        ///     service could not be resolved.
        /// </summary>
        ThrowException,

        /// <summary>
        ///     Denotes that the resolver should return the default value for the service type if the
        ///     service could not be resolved.
        /// </summary>
        ReturnDefault
    }
}