namespace Butler.Resolver
{
    /// <summary>
    ///     A set of different service construction modes.
    /// </summary>
    public enum ServiceConstructionMode
    {
        /// <summary>
        ///     Denotes that the default <see cref="ServiceConstructionMode"/> should be used for the
        ///     current context.
        /// </summary>
        Default,

        /// <summary>
        ///     Denotes that the <see cref="ServiceConstructionMode"/> should be inherited from the
        ///     parent resolve, e.g. when resolving a dependency of a service and the parent mode is
        ///     set, then the <see cref="ServiceConstructionMode"/> of the parent resolve is used. If
        ///     the service is resolved directly and <see cref="Parent"/> is set then
        ///     <see cref="Default"/> is used.
        /// </summary>
        Parent,

        /// <summary>
        ///     Denotes that the parameter-less constructor should be preferred.
        /// </summary>
        PreferParameterlessConstructor,

        /// <summary>
        ///     Denotes that the most complex constructor should be searched that can be created with
        ///     the current registrations.
        /// </summary>
        PreferComplexConstructor,

        /// <summary>
        ///     Denotes that the constructor with the <see cref="PreferConstructorAttribute"/> should
        ///     be used. If no constructor has the <see cref="PreferConstructorAttribute"/> then the
        ///     same behavior as <see cref="PreferComplexConstructor"/> is used.
        /// </summary>
        Mixed
    }
}