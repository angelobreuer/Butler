namespace Butler.Lifetime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Butler.Resolver;

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     Implementation of an <see cref="ILifetimeManager"/> that creates a service only once
    ///     regardless of the request scope.
    /// </summary>
    public sealed class SingletonLifetimeManager : ILifetimeManager,
#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else // SUPPORTS_ASYNC_DISPOSABLE
        IDisposable
#endif // !SUPPORTS_ASYNC_DISPOSABLE
    {
        /// <summary>
        ///     A dictionary containing the service instance keyed by the service type.
        /// </summary>
        private readonly IDictionary<Type, object> _services;

#if SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A <see cref="SemaphoreSlim"/> for the <see cref="_services"/>.
        /// </summary>
        private readonly SemaphoreSlim _trackerLock;
#else // SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A lock <see cref="object"/> for the <see cref="_services"/>.
        /// </summary>
        private readonly object _serviceLock;

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A value indicating whether the lifetime manager was disposed (
        ///     <see langword="true"/>); otherwise <see langword="false"/>.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingletonLifetimeManager"/> class.
        /// </summary>
        public SingletonLifetimeManager()
        {
            _services = new Dictionary<Type, object>();

#if SUPPORTS_ASYNC_DISPOSABLE
            _trackerLock = new SemaphoreSlim(1, 1);
#else // SUPPORTS_ASYNC_DISPOSABLE
            _serviceLock = new object();
#endif // !SUPPORTS_ASYNC_DISPOSABLE
        }

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes all tracked objects asynchronously.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                // the lifetime manager has been already disposed
                return;
            }

            // set disposed flag
            _disposed = true;

            // acquire lock
            await _trackerLock.WaitAsync();

            // ensure the lock is released even if an exception is thrown
            try
            {
                // iterate through all tracked objects
                foreach (var instance in _services.Values)
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

                // clear tracked instances
                _services.Clear();
            }
            finally
            {
                // release lock
                _trackerLock.Release();
            }

            // dispose lock
            _trackerLock.Dispose();
        }
#else // SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Disposes all tracked objects.
        /// </summary>
        public void Dispose()
        {
            // check if the lifetime manager was already disposed
            if (_disposed)
            {
                return;
            }

            // set disposed flag
            _disposed = true;

            // acquire lock
            lock (_serviceLock)
            {
                // iterate through all tracked objects
                foreach (var instance in _services.Values)
                {
                    // check if the instance inherits from the IDisposable interface
                    if (instance is IDisposable disposable)
                    {
                        // dispose instance
                        disposable.Dispose();
                    }
                }

                // clear tracked instances
                _services.Clear();
            }
        }

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Tries to resolve an object from the manager.
        /// </summary>
        /// <param name="resolveContext">the current resolver context</param>
        /// <param name="scopeKey">
        ///     the <paramref name="scopeKey"/> the instance was created for; if
        ///     <see langword="null"/> then the scope is global.
        /// </param>
        /// <returns>the resolved service; or default if the service could not be resolved</returns>
        public object Resolve(ServiceResolveContext resolveContext, object scopeKey = null)
        {
            // create the key for resolving the type from the scope
            return _services.TryGetValue(resolveContext.ServiceType, out var service) ? service : default;
        }

        /// <summary>
        ///     Tracks the specified <paramref name="instance"/> for disposation.
        /// </summary>
        /// <param name="resolveContext">the resolver context from which the service was resolved</param>
        /// <param name="instance">the instance to track</param>
        /// <param name="scopeKey">
        ///     the <paramref name="scopeKey"/> the instance was created for; if
        ///     <see langword="null"/> then the scope is global.
        /// </param>
        public void TrackInstance(ServiceResolveContext resolveContext, object instance, object scopeKey = null)
        {
#if SUPPORTS_ASYNC_DISPOSABLE
            // acquire lock
            _trackerLock.Wait();

            // ensure the lock is released even if an exception is thrown
            try
            {
                // track instance
                _services.Add(resolveContext.ServiceType, instance);
            }
            finally
            {
                // release lock and let other threads continue
                _trackerLock.Release();
            }
#else // SUPPORTS_ASYNC_DISPOSABLE
            lock (_serviceLock)
            {
                // add instance to tracking list
                _services.Add(resolveContext.ServiceType, instance);
            }
#endif // !SUPPORTS_ASYNC_DISPOSABLE
        }
    }
}