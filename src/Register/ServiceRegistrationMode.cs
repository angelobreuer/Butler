namespace Butler.Register
{
    /// <summary>
    ///     A set of different service registration modes.
    /// </summary>
    public enum ServiceRegistrationMode : byte
    {
        /// <summary>
        ///     Denotes that an exception should be thrown if a service registration already exists
        ///     for the specified service type.
        /// </summary>
        Default,

        /// <summary>
        ///     Denotes that the existing service registration should be replaced with the new.
        /// </summary>
        Replace,

        /// <summary>
        ///     Denotes that the existing service registration should be kept and the new service
        ///     registration should be dropped without registering.
        /// </summary>
        Ignore,

        /// <summary>
        ///     Denotes that the existing service registration should be converted to a
        ///     multi-registration. If no registration exists then the registration is kept.
        /// </summary>
        Append
    }
}