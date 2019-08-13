namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Butler.Register;
    using Butler.Registration;

    /// <summary>
    ///     Interface for a root container.
    /// </summary>
    public interface IRootContainer : IEnumerable<KeyValuePair<Type, IServiceRegistration>>, IEnumerable, IServiceRegister,
#if SUPPORTS_SERVICE_PROVIDER
        IServiceProvider,
#endif
#if SUPPORTS_ASYNC_DISPOSABLE
        IAsyncDisposable
#else
        IDisposable
#endif
    {
    }
}