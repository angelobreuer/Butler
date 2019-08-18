namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Registration;
    using Butler.Resolver;

#if DEBUG
    using System.Diagnostics;
    using Butler.Util;
    using Butler.Util.Debug;
    using Butler.Lifetime;
#endif // DEBUG

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
#if DEBUG

    [DebuggerTypeProxy(typeof(RootContainerProxy))]
#endif // DEBUG
    public class RootContainer : BaseServiceResolver, IRootContainer, IServiceResolver
    {
        /// <summary>
        ///     A dictionary containing service lifetimes storing an <see cref="ILifetimeManager"/> class.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private readonly IDictionary<IServiceLifetime, ILifetimeManager> _lifetimes; // TODO make thread-safe

        /// <summary>
        ///     A value indicating whether the root container has been disposed.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private volatile bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootContainer"/> class.
        /// </summary>
        public RootContainer()
        {
            _lifetimes = new Dictionary<IServiceLifetime, ILifetimeManager>();
        }

#if !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Disposes the root container.
        /// </summary>
        public void Dispose()
        {
            // check if the container was already disposed
            if (_disposed)
            {
                return;
            }

            // set disposed flag
            _disposed = true;

            // iterate through all used lifetimes and dispose them
            foreach (var lifetime in _lifetimes.Values)
            {
                // dispose lifetime
                lifetime.Dispose();
            }
        }

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Gets the service registration enumerator.
        /// </summary>
        /// <returns>the service registration enumerator</returns>
        public IEnumerator<KeyValuePair<Type, IServiceRegistration>> GetEnumerator()
            => Registrations.GetEnumerator();

        /// <summary>
        ///     Gets the service registration enumerator.
        /// </summary>
        /// <returns>the service registration enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="scopeKey">
        ///     the scope key for resolving the service; if <see langword="null"/> the global scope
        ///     is used.
        /// </param>
        /// <param name="parentContext">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        protected override object ResolveService(Type serviceType, object scopeKey = null,
            ServiceResolveContext parentContext = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
        {
            // check if the container is already disposed
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(RootContainer));
            }

            // null-check service type
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType), "Could not resolve service of type <null>.");
            }

            // create a context for this resolve
            var context = parentContext == null ?
                new ServiceResolveContext(this, this, constructionMode, serviceType) :
                new ServiceResolveContext(parentContext, constructionMode, serviceType);
#if DEBUG
            // trace resolve
            context.TraceBuilder.AppendResolve(serviceType);
#endif // DEBUG

            // find registration
            var registration = FindRegistration(serviceType);

            // check if the registration failed
            if (registration is null)
            {
                // throw resolver exception
#if DEBUG
                context.TraceBuilder.AppendResolveFail(serviceType, critical: true);
#endif // DEBUG

                throw new ResolverException($"Could not resolve service of type '{serviceType}' (No registration).", context);
            }

            // try getting the lifetime manager
            if (!_lifetimes.TryGetValue(registration.ServiceLifetime, out var lifetimeManager))
            {
                // create lifetime manager for this resolver
                lifetimeManager = registration.ServiceLifetime.CreateManager(this);

                // add the lifetime manager
                _lifetimes.Add(registration.ServiceLifetime, lifetimeManager);
            }

            // try resolving from the lifetime
            var lifetimeObject = lifetimeManager.Resolve(context, scopeKey);

            // check if the object is cached
            if (lifetimeObject != null)
            {
                // return cached service
                return lifetimeObject;
            }

            // create service instance
            var service = registration.Create(context);

            // track service
            lifetimeManager.TrackInstance(context, service, scopeKey);

            return service;
        }

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes the root container asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                // root container already disposed
                return;
            }

            // set disposed flag
            _disposed = true;

            // iterate through all used lifetimes and dispose them
            foreach (var lifetime in _lifetimes.Values)
            {
                // dispose lifetime
                await lifetime.DisposeAsync();
            }
        }
#endif // SUPPORTS_ASYNC_DISPOSABLE
    }
}