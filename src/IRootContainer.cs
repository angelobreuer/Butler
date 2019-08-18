namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Register;
    using Butler.Registration;
    using Butler.Resolver;

    /// <summary>
    ///     Interface for a root container.
    /// </summary>
    public interface IRootContainer : IEnumerable<KeyValuePair<Type, IServiceRegistration>>, IEnumerable, IServiceRegister, IServiceResolver,
#if SUPPORTS_SERVICE_PROVIDER
        IServiceProvider,
#endif // SUPPORTS_SERVICE_PROVIDER

#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else
        IDisposable
#endif // SUPPORTS_ASYNC_DISPOSABLE
    {
        /// <summary>
        ///     Gets or sets the container service resolution mode.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified value is not defined in the
        ///     <see cref="ContainerResolveMode"/> enumeration.
        /// </exception>
        ContainerResolveMode ContainerResolveMode { get; set; }

        /// <summary>
        ///     Gets the parent container (if <see langword="null"/> then the container has no parent).
        /// </summary>
        IRootContainer Parent { get; }

        /// <summary>
        ///     Creates a new child container of the current <see cref="IRootContainer"/>.
        /// </summary>
        /// <returns>the child container</returns>
        IRootContainer CreateChild();
    }
}