#if !NETSTANDARD2_0

/*
 * The IServiceProvider came available in .NET Standard 2.0.
 * https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0_diff.md
 */

namespace System
{
    /// <summary>
    ///     A fallback interface for service providers.
    /// </summary>
    public interface IServiceProvider
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
        object GetService(Type serviceType);
    }
}

#endif