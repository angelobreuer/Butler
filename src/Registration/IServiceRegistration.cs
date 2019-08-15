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
        /// <param name="resolver">the calling resolver</param>
        /// <returns>the instance</returns>
        object Create(IServiceResolver resolver);
    }
}