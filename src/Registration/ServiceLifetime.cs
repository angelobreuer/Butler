namespace Butler.Registration
{
    /// <summary>
    ///     A set of different service lifetimes.
    /// </summary>
    public enum ServiceLifetime : byte
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