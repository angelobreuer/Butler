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
    }
}