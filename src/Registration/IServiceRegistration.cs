namespace Butler.Registration
{
    public interface IServiceRegistration
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <returns>the instance</returns>
        object Create();
    }
}