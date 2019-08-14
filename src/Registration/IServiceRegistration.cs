namespace Butler.Registration
{
    /// <summary>
    ///     Interface for basic service registrations.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <returns>the instance</returns>
        object Create();
    }
}