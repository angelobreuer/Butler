namespace Butler.Registration
{
    /// <summary>
    ///     A set of different registration types.
    /// </summary>
    public enum RegistrationType : byte
    {
        /// <summary>
        ///     Denotes that each resolve the same instance is used.
        /// </summary>
        Singleton,

        /// <summary>
        ///     Denotes that each resolve in the same scope is created new.
        /// </summary>
        Scoped,

        /// <summary>
        ///     Denotes that each resolve the instance is created new.
        /// </summary>
        Transient
    }
}