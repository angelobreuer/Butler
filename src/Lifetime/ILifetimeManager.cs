namespace Butler.Lifetime
{
    using System;

    /// <summary>
    ///     Interface for lifetime managers.
    /// </summary>
    public interface ILifetimeManager :
#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else // SUPPORTS_ASYNC_DISPOSABLE
        IDisposable
#endif // !SUPPORTS_ASYNC_DISPOSABLE
    {
        /// <summary>
        ///     Tries to resolve an object from the manager.
        /// </summary>
        /// <param name="serviceType">the type of the service being resolved</param>
        /// <param name="scope">
        ///     the service scope; if <see langword="null"/> then the scope is global
        /// </param>
        /// <returns>the resolved service; or default if the service could not be resolved</returns>
        object Resolve(Type serviceType, object scope = null);

        /// <summary>
        ///     Tracks the specified <paramref name="instance"/> for disposation.
        /// </summary>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="scope">the <paramref name="scope"/> the instance was created for</param>
        /// <param name="instance">the instance to track</param>
        void TrackInstance(Type serviceType, object scope, object instance);
    }
}