namespace Butler.Register
{
    using System;

    /// <summary>
    ///     An exception for service registration errors.
    /// </summary>
#if SUPPORTS_SERIALIZABLE
    [Serializable]
#endif

    public sealed class RegistrationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RegistrationException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        internal RegistrationException(string message) : base(message)
        {
        }
    }
}