namespace Butler.Lifetime
{
    using System;
    using System.Collections.Generic;

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     Implementation of an <see cref="ILifetimeManager"/> that re-creates services each call.
    /// </summary>
    public sealed class TransientLifetimeManager : ILifetimeManager,
#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else // SUPPORTS_ASYNC_DISPOSABLE
        IDisposable
#endif // !SUPPORTS_ASYNC_DISPOSABLE
    {
#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     A list holding all (asynchronously) disposable instances the lifetime created. The
        ///     list either holds an <see cref="IAsyncDisposable"/> or an <see cref="IDisposable"/>.
        ///     Created instances which do not implement any of <see cref="IAsyncDisposable"/> or
        ///     <see cref="IDisposable"/> are not tracked.
        /// </summary>
        private readonly IList<object> _tracker;
#else // SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     A list holding all disposable instances the lifetime created. The list only holds
        ///     <see cref="IDisposable"/> instances.
        /// </summary>
        private readonly IList<IDisposable> _tracker;

#endif // !SUPPORTS_ASYNC_DISPOSABLE

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes all tracked objects asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            // iterate through all tracked objects
            foreach (var instance in _tracker)
            {
                // check if the instance implements IAsyncDisposable
                if (instance is IAsyncDisposable asyncDisposable)
                {
                    // dispose instance asynchronously
                    await asyncDisposable.DisposeAsync();
                }

                // check if the instance implements IDisposable
                else if (instance is IDisposable disposable)
                {
                    // dispose instance
                    disposable.Dispose();
                }
            }
        }
#else // SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Disposes all tracked objects.
        /// </summary>
        public void Dispose()
        {
            // iterate through all tracked objects
            foreach (var instance in _tracker)
            {
                // dispose instance
                instance.Dispose();
            }
        }

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Tries to resolve an object from the manager.
        /// </summary>
        /// <param name="serviceType">the type of the service being resolved</param>
        /// <param name="scope">
        ///     the service scope; if <see langword="null"/> then the scope is global
        /// </param>
        /// <returns>the resolved service; or default if the service could not be resolved</returns>
        public object Resolve(Type serviceType, object scope = null)
        {
            // The transient lifetime always re-creates services.
            return null;
        }

        /// <summary>
        ///     Tracks the specified <paramref name="instance"/> for disposation.
        /// </summary>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="scope">the <paramref name="scope"/> the instance was created for</param>
        /// <param name="instance">the instance to track</param>
        public void TrackInstance(Type serviceType, object scope, object instance)
        {
            // The instances in the lifetime are tracked to dispose them, so we need only to track
            // disposable / asynchronously disposable objects.
#if SUPPORTS_ASYNC_DISPOSABLE
            if (instance is IDisposable || instance is IAsyncDisposable)
            {
                _tracker.Add(instance);
            }
#else // SUPPORTS_ASYNC_DISPOSABLE
            if (instance is IDisposable disposable)
            {
                // add instance to tracking list
                _tracker.Add(disposable);
            }
#endif // !SUPPORTS_ASYNC_DISPOSABLE
        }
    }
}