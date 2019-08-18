namespace Butler.Lifetime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

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
        /// <summary>
        ///     A list holding all tracked service instances.
        /// </summary>
        private readonly IList<object> _tracker;

#if SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A <see cref="SemaphoreSlim"/> for the <see cref="_tracker"/>.
        /// </summary>
        private readonly SemaphoreSlim _trackerLock;
#else // SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A lock <see cref="object"/> for the <see cref="_tracker"/>.
        /// </summary>
        private readonly object _trackerLock;

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     A value indicating whether the lifetime manager was disposed (
        ///     <see langword="true"/>); otherwise <see langword="false"/>.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransientLifetimeManager"/> class.
        /// </summary>
        public TransientLifetimeManager()
        {
            _tracker = new List<object>();

#if SUPPORTS_ASYNC_DISPOSABLE
            _trackerLock = new SemaphoreSlim(1, 1);
#else // SUPPORTS_ASYNC_DISPOSABLE
            _trackerLock = new object();
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

                // clear tracked instances
                _tracker.Clear();
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
            lock (_trackerLock)
            {
                // iterate through all tracked objects
                foreach (var instance in _tracker)
                {
                    // check if the instance inherits from the IDisposable interface
                    if (instance is IDisposable disposable)
                    {
                        // dispose instance
                        disposable.Dispose();
                    }
                }

                // clear tracked instances
                _tracker.Clear();
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
            // check if the instance does not inherit from IDisposable or IAsyncDisposable, then
            // there is no need to track the service instance since the transient lifetime always
            // re-creates services
            if (!(instance is IDisposable || instance is IAsyncDisposable))
            {
                // there is no need to track the instance
                return;
            }

            // acquire lock
            _trackerLock.Wait();

            // ensure the lock is released even if an exception is thrown
            try
            {
                // track instance
                _tracker.Add(instance);
            }
            finally
            {
                // release lock and let other threads continue
                _trackerLock.Release();
            }
#else // SUPPORTS_ASYNC_DISPOSABLE
            if (instance is IDisposable disposable)
            {
                lock (_trackerLock)
                {
                    // add instance to tracking list
                    _tracker.Add(disposable);
                }
            }
#endif // !SUPPORTS_ASYNC_DISPOSABLE
        }
    }
}