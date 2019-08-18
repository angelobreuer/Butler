#if DEBUG

namespace Butler.Util.Proxy
{
    using System;
    using System.Diagnostics;
    using Butler.Resolver;

    /// <summary>
    ///     A class for wrapping the resolver settings into a category.
    /// </summary>
    internal sealed class ResolverSettingsProxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverSettingsProxy"/> class.
        /// </summary>
        /// <param name="serviceResolver">the service resolver bound to</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceResolver"/> is <see langword="null"/>.
        /// </exception>
        public ResolverSettingsProxy(IServiceResolver serviceResolver)
            => ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        /// <summary>
        ///     Gets the service resolver bound to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IServiceResolver ServiceResolver { get; }

        /// <summary>
        ///     Gets or sets the maximum depth of the <see cref="ServiceResolver"/>.
        /// </summary>
        public int MaximumDepth
        {
            get => ServiceResolver.MaximumDepth;
            set => ServiceResolver.MaximumDepth = value;
        }

        /// <summary>
        ///     Gets or sets the service construction mode of the <see cref="ServiceResolver"/>.
        /// </summary>
        public ServiceConstructionMode ServiceConstructionMode
        {
            get => ServiceResolver.ServiceConstructionMode;
            set => ServiceResolver.ServiceConstructionMode = value;
        }
    }
}

#endif // DEBUG