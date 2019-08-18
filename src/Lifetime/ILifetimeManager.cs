namespace Butler.Lifetime
{
    using System;
    using Butler.Resolver;

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
        /// <param name="resolveContext">the current resolver context</param>
        /// <param name="scopeKey">
        ///     the <paramref name="scopeKey"/> the instance was created for; if
        ///     <see langword="null"/> then the scope is global.
        /// </param>
        /// <returns>the resolved service; or default if the service could not be resolved</returns>
        object Resolve(ServiceResolveContext resolveContext, object scopeKey = null);

        /// <summary>
        ///     Tracks the specified <paramref name="instance"/> for disposation.
        /// </summary>
        /// <param name="resolveContext">the resolver context from which the service was resolved</param>
        /// <param name="instance">the instance to track</param>
        /// <param name="scopeKey">
        ///     the <paramref name="scopeKey"/> the instance was created for; if
        ///     <see langword="null"/> then the scope is global.
        /// </param>
        void TrackInstance(ServiceResolveContext resolveContext, object instance, object scopeKey = null);
    }
}