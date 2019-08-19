#if DEBUG

namespace Butler.Util.Proxy
{
    using System;
    using System.Diagnostics;
    using Butler.Lifetime;

    /// <summary>
    ///     Debugger Type Proxy for <see cref="IServiceLifetime"/>.
    /// </summary>
    [DebuggerDisplay("{LifetimeManager.GetType().Name}", Name = "{ServiceLifetime.Name}")]
    internal sealed class LifetimeProxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LifetimeProxy"/> class.
        /// </summary>
        /// <param name="serviceLifetime">the service lifetime bound to</param>
        /// <param name="lifetimeManager">the lifetime manager bound to</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceLifetime"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="lifetimeManager"/> is <see langword="null"/>.
        /// </exception>
        public LifetimeProxy(IServiceLifetime serviceLifetime, ILifetimeManager lifetimeManager)
        {
            ServiceLifetime = serviceLifetime ?? throw new ArgumentNullException(nameof(serviceLifetime));
            LifetimeManager = lifetimeManager ?? throw new ArgumentNullException(nameof(lifetimeManager));
        }

        /// <summary>
        ///     Gets the service lifetime bound to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IServiceLifetime ServiceLifetime { get; }

        /// <summary>
        ///     Gets the lifetime manager bound to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ILifetimeManager LifetimeManager { get; }
    }
}

#endif // DEBUG