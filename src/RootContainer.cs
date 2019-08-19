namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Registration;
    using Butler.Resolver;
    using Butler.Util;
    using Butler.Util.Proxy;
    using Butler.Lifetime;

#if DEBUG
    using System.Diagnostics;
#endif // DEBUG

#if !SUPPORTS_CONCURRENT_COLLECTIONS && SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading;
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS && SUPPORTS_ASYNC_DISPOSABLE

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

#if SUPPORTS_CONCURRENT_COLLECTIONS
    using System.Collections.Concurrent;
#endif // SUPPORTS_CONCURRENT_COLLECTIONS

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
#if DEBUG

    [DebuggerTypeProxy(typeof(RootContainerProxy))]
#endif // DEBUG
    public class RootContainer : BaseServiceResolver, IRootContainer, IServiceResolver
    {
        /// <summary>
        ///     The parent container (if <see langword="null"/>, then this container has no parent).
        /// </summary>
        private readonly RootContainer _parent;

        /// <summary>
        ///     A dictionary containing service lifetimes storing an <see cref="ILifetimeManager"/> class.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG

#if SUPPORTS_CONCURRENT_COLLECTIONS
        private readonly ConcurrentDictionary<IServiceLifetime, ILifetimeManager> _lifetimes;
#else // SUPPORTS_CONCURRENT_COLLECTIONS
        private readonly IDictionary<IServiceLifetime, ILifetimeManager> _lifetimes;

        /// <summary>
        ///     The lock used for the lifetime manager dictionary ( <see cref="_lifetimes"/>).
        /// </summary>
#if SUPPORTS_ASYNC_DISPOSABLE
        private readonly SemaphoreSlim _lifetimeLock;
#else // SUPPORTS_ASYNC_DISPOSABLE
        private readonly object _lifetimeLock;
#endif // !SUPPORTS_ASYNC_DISPOSABLE

#endif // !SUPPORTS_CONCURRENT_COLLECTIONS

        /// <summary>
        ///     A value indicating whether the root container has been disposed.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private volatile bool _disposed;

        /// <summary>
        ///     The actual <see cref="ContainerResolveMode"/> for the container.
        /// </summary>
#if DEBUG
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif // DEBUG
        private ContainerResolveMode _containerResolveMode = DefaultContainerResolveMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootContainer"/> class.
        /// </summary>
        /// <param name="parent">the parent container</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="parent"/> is <see langword="null"/>.
        /// </exception>
        public RootContainer(RootContainer parent) : this()
            => _parent = parent ?? throw new ArgumentNullException(nameof(parent));

        /// <summary>
        ///     Initializes a new instance of the <see cref="RootContainer"/> class.
        /// </summary>
        public RootContainer()
        {
#if SUPPORTS_CONCURRENT_COLLECTIONS
            _lifetimes = new ConcurrentDictionary<IServiceLifetime, ILifetimeManager>();
#else // SUPPORTS_CONCURRENT_DICTIONARY
            _lifetimes = new Dictionary<IServiceLifetime, ILifetimeManager>();
#if SUPPORTS_ASYNC_DISPOSABLE
            _lifetimeLock = new SemaphoreSlim(1, 1);
#else // SUPPORTS_ASYNC_DISPOSABLE
            _lifetimeLock = new object();
#endif // !SUPPORTS_ASYNC_DISPOSABLE
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS
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

#if !SUPPORTS_CONCURRENT_COLLECTIONS
            // acquire lock
            lock (_lifetimeLock)
            {
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS
                // iterate through all used lifetimes and dispose them
                foreach (var lifetime in _lifetimes.Values)
                {
                    // dispose lifetime
                    lifetime.Dispose();
                }
#if !SUPPORTS_CONCURRENT_COLLECTIONS
            }
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS
        }

#endif // !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Gets the parent container (if <see langword="null"/> then the container has no parent).
        /// </summary>
        public IRootContainer Parent => _parent;

        /// <summary>
        ///     The default value for the <see cref="ContainerResolveMode"/> property.
        /// </summary>
        public const ContainerResolveMode DefaultContainerResolveMode
            = ContainerResolveMode.ChildFirst;

        /// <summary>
        ///     Gets or sets the container service resolution mode.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the
        ///     <see cref="ContainerResolveMode"/> enumeration.
        /// </exception>
        public ContainerResolveMode ContainerResolveMode
        {
            get => _containerResolveMode;

            set
            {
                // ensure the mode is defined
                if (!Enum.IsDefined(typeof(ContainerResolveMode), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "The specified value is not defined in the ContainerResolveMode enumeration.");
                }

                _containerResolveMode = value;
            }
        }

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
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="IServiceResolver.DefaultResolveMode"/> is used.
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
        protected override object ResolveService(Type serviceType, object scopeKey = null, ServiceResolveContext context = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
        {
            // check if the container is parent-less
            if (_parent is null)
            {
                // resolve the service using the default behavior
                return ResolveServiceInternal(serviceType, scopeKey, context, resolveMode, constructionMode);
            }

            // resolve using the first choice
            var firstChoice = ContainerResolveMode == ContainerResolveMode.ChildFirst ?
                ResolveServiceInternal(serviceType, scopeKey, context, ServiceResolveMode.ReturnDefault, constructionMode) :
                _parent.ResolveServiceInternal(serviceType, scopeKey, context, ServiceResolveMode.ReturnDefault, constructionMode);

            // check if the first choice could resolve the service
            if (firstChoice != default)
            {
                return firstChoice;
            }

            // fallback to next container
            return ContainerResolveMode == ContainerResolveMode.ParentFirst ?
                ResolveServiceInternal(serviceType, scopeKey, context, resolveMode, constructionMode) :
                _parent.ResolveServiceInternal(serviceType, scopeKey, context, resolveMode, constructionMode);
        }

        /// <summary>
        ///     Creates a new child container of the current <see cref="IRootContainer"/>.
        /// </summary>
        /// <returns>the child container</returns>
        public IRootContainer CreateChild()
            => new RootContainer(this);

        /// <summary>
        ///     Resolves a registration for the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="IServiceResolver.DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="registration">
        ///     the registration found; or <see langword="null"/> if the result value is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     a value indicating whether the registration was found ( <see langword="true"/>),
        ///     otherwise <see langword="false"/> if the registration was not found and the specified
        ///     <paramref name="resolveMode"/> is <see cref="ServiceResolveMode.ReturnDefault"/> (or
        ///     the specified <paramref name="resolveMode"/> is
        ///     <see cref="ServiceResolveMode.Default"/> and the resolver
        ///     <see cref="IServiceResolver.DefaultResolveMode"/> is <see cref="ServiceResolveMode.ReturnDefault"/>)
        /// </returns>
        private bool ResolveRegistration(ServiceResolveContext context, ServiceResolveMode resolveMode, out IServiceRegistration registration)
        {
            // find registration
            registration = FindRegistration(context.ServiceType);

            // check if the registration failed
            if (registration is null)
            {
                // check if the default service resolve mode should be used
                if (resolveMode == ServiceResolveMode.Default)
                {
                    // use default resolve mode
                    resolveMode = DefaultResolveMode;
                }

                // check if an exception should be thrown
                if (resolveMode == ServiceResolveMode.ThrowException)
                {
                    // throw resolver exception
#if DEBUG
                    context.TraceBuilder.AppendResolveFail(context.ServiceType, critical: true);
#endif // DEBUG

                    throw new ResolverException($"Could not resolve service of type '{context.ServiceType}' (No registration).", context);
                }

                // The ServiceResolveMode is ReturnDefault.
                return false;
            }

            return true;
        }

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
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="IServiceResolver.DefaultResolveMode"/> is used.
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
        private object ResolveServiceInternal(Type serviceType, object scopeKey = null,
            ServiceResolveContext parentContext = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
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

            // resolve the service registration
            if (!ResolveRegistration(context, resolveMode, out var registration))
            {
                return default;
            }

            return Resolve(registration, context, scopeKey);
        }

        /// <summary>
        ///     Resolves a service directly from the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to resolve the service from</param>
        /// <param name="context">the service resolver context</param>
        /// <param name="scopeKey">
        ///     the current scope key; or <see langword="null"/> to use the global scope
        /// </param>
        /// <returns>the resolved service</returns>
        public override object Resolve(IServiceRegistration registration, ServiceResolveContext context, object scopeKey = null)
        {
            // the manager for the lifetime
            var lifetimeManager = GetLifetimeManager(registration.ServiceLifetime);

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

        /// <summary>
        ///     Retrieves the <see cref="ILifetimeManager"/> for the specified <paramref name="serviceLifetime"/>.
        /// </summary>
        /// <param name="serviceLifetime">the service lifetime to get the manager for</param>
        /// <returns>the lifetime manager</returns>
        private ILifetimeManager GetLifetimeManager(IServiceLifetime serviceLifetime)
        {
#if SUPPORTS_CONCURRENT_COLLECTIONS

            // get the corresponding lifetime manager
            return _lifetimes.GetOrAdd(serviceLifetime,
                lifetime => lifetime.CreateManager(this));

#else // SUPPORTS_CONCURRENT_COLLECTIONS

#if SUPPORTS_ASYNC_DISPOSABLE

            // acquire lock
            _lifetimeLock.Wait();

            // make sure the lock is released even if an exception is thrown
            try
            {
#else // SUPPORTS_ASYNC_DISPOSABLE
            lock (_lifetimeLock)
            {
#endif // !SUPPORTS_ASYNC_DISPOSABLE

                // try getting the lifetime manager
                if (!_lifetimes.TryGetValue(serviceLifetime, out var lifetimeManager))
                {
                    // create lifetime manager for this resolver
                    lifetimeManager = serviceLifetime.CreateManager(this);

                    // add the lifetime manager
                    _lifetimes.Add(serviceLifetime, lifetimeManager);
                }

                return lifetimeManager;

#if SUPPORTS_ASYNC_DISPOSABLE
            }
            finally
            {
                // release lock
                _lifetimeLock.Release();
            }
#else // SUPPORTS_ASYNC_DISPOSABLE
            }
#endif // !SUPPORTS_ASYNC_DISPOSABLE

#endif // !SUPPORTS_CONCURRENT_COLLECTIONS
        }

        /// <summary>
        ///     Resolves multiple services of the specified <paramref name="serviceType"/>.
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
        /// <param name="resolveMode">
        ///     the service resolution mode; if <see cref="ServiceResolveMode.Default"/> then the
        ///     <see cref="IServiceResolver.DefaultResolveMode"/> is used.
        /// </param>
        /// <param name="constructionMode">
        ///     the service construction mode; which defines the behavior for resolving constructors
        ///     for a service implementation type.
        /// </param>
        /// <returns>
        ///     an enumerable that enumerates through the services. The service enumerable caches the
        ///     service creations and creates the service as iterated (if not already cached).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        /// <exception cref="ObjectDisposedException">thrown if the container is disposed.</exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the maximum service resolve depth was exceeded.
        /// </exception>
        public override IEnumerable ResolveAll(Type serviceType, object scopeKey = null, ServiceResolveContext parentContext = null,
            ServiceResolveMode resolveMode = ServiceResolveMode.Default,
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

            // resolve the service registration
            if (!ResolveRegistration(context, resolveMode, out var registration))
            {
                return default;
            }

            // check if the registration is not a multi registration
            if (!(registration is MultiRegistration multiRegistration))
            {
                // resolve the service normally and return it as a single-item enumerable
                return new[] { Resolve(registration, context, scopeKey) };
            }

            return multiRegistration.Create(context);
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

#if !SUPPORTS_CONCURRENT_COLLECTIONS

            // acquire lock
            await _lifetimeLock.WaitAsync();

            // make sure the lock is released even if an exception is thrown
            try
            {
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS

            // iterate through all used lifetimes and dispose them
            foreach (var lifetime in _lifetimes.Values)
            {
                // dispose lifetime
                await lifetime.DisposeAsync();
            }

#if !SUPPORTS_CONCURRENT_COLLECTIONS
            }
            finally
            {
                // release lock
                _lifetimeLock.Release();
            }

            // dispose lock
            _lifetimeLock.Dispose();
#endif // !SUPPORTS_CONCURRENT_COLLECTIONS
        }
#endif // SUPPORTS_ASYNC_DISPOSABLE
    }
}