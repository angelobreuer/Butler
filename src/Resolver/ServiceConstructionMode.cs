namespace Butler.Resolver
{
    /// <summary>
    ///     A set of different service construction modes.
    /// </summary>
    public enum ServiceConstructionMode
    {
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