namespace Butler
{
    using System;

    /// <summary>
    ///     A inversion of control (IoC) container that supports resolving services.
    /// </summary>
    public class RootContainer : IServiceProvider
    {
        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type to resolve</param>
        /// <returns>
        ///     the resolved service; or <see langword="null"/> if the service could not been resolved
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        public object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            // TODO
            return null;
        }
    }
}