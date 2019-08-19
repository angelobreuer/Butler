#if DEBUG

namespace Butler.Util.Proxy
{
    using System;
    using System.Diagnostics;
    using Butler.Lifetime;
    using Butler.Register;
    using Butler.Resolver;

    /// <summary>
    ///     A class for wrapping the container settings into a category.
    /// </summary>
    internal sealed class SettingsProxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsProxy"/> class.
        /// </summary>
        /// <param name="rootContainer">the root container bound to</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="rootContainer"/> is <see langword="null"/>.
        /// </exception>
        public SettingsProxy(IRootContainer rootContainer)
            => RootContainer = rootContainer ?? throw new ArgumentNullException(nameof(rootContainer));

        /// <summary>
        ///     Gets or sets the default service registration mode.
        /// </summary>
        public ServiceRegistrationMode DefaultRegistrationMode
        {
            get => RootContainer.DefaultRegistrationMode;
            set => RootContainer.DefaultRegistrationMode = value;
        }

        /// <summary>
        ///     Gets or sets the default <see cref="ServiceResolveMode"/> that is used when
        ///     <see cref="ServiceResolveMode.Default"/> is passed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the <see cref="ServiceResolveMode"/> enumeration.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is <see cref="ServiceResolveMode.Default"/> (this mode
        ///     can not be used, because the resolver specifies the default value, this mode can only
        ///     be used when passing to a resolver method)
        /// </exception>
        public ServiceResolveMode DefaultResolveMode
        {
            get => RootContainer.DefaultResolveMode;
            set => RootContainer.DefaultResolveMode = value;
        }

        /// <summary>
        ///     Gets or sets the default service lifetime when no specific lifetime was specified.
        /// </summary>
        public IServiceLifetime DefaultServiceLifetime
        {
            get => RootContainer.DefaultServiceLifetime;
            set => RootContainer.DefaultServiceLifetime = value;
        }

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly => RootContainer.IsReadOnly;

        /// <summary>
        ///     Gets or sets the maximum depth of the <see cref="RootContainer"/>.
        /// </summary>
        public int MaximumDepth
        {
            get => RootContainer.MaximumDepth;
            set => RootContainer.MaximumDepth = value;
        }

        /// <summary>
        ///     Gets the root container bound to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IRootContainer RootContainer { get; }

        /// <summary>
        ///     Gets or sets the service construction mode of the <see cref="RootContainer"/>.
        /// </summary>
        public ServiceConstructionMode ServiceConstructionMode
        {
            get => RootContainer.ServiceConstructionMode;
            set => RootContainer.ServiceConstructionMode = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether disposable transients should be tracked by
        ///     the container. Note that enabling this property can cause memory leaks, because the
        ///     transients are released when the container is disposed.
        /// </summary>
        public bool TrackDisposableTransients
        {
            get => RootContainer.TrackDisposableTransients;
            set => RootContainer.TrackDisposableTransients = value;
        }
    }
}

#endif // DEBUG