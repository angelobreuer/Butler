namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Butler.Registration;
    using Butler.Resolver;

#if DEBUG

    using Butler.Util;

#endif // DEBUG

#if SUPPORTS_ASYNC_DISPOSABLE
    using System.Threading.Tasks;
#endif // SUPPORTS_ASYNC_DISPOSABLE

    /// <summary>
    ///     An inversion of control (IoC) container that supports resolving services.
    /// </summary>
    public class RootContainer : BaseServiceResolver, IRootContainer, IServiceResolver
    {
        /// <summary>
        ///     A value indicating whether the root container has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Finalizes the <see cref="RootContainer"/> instance.
        /// </summary>
        ~RootContainer()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Disposes the root container.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        public override object Resolve(Type serviceType, ServiceResolveContext parentContext = null,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Default)
        {
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

            // TODO TODO TODO
            return registration.Create(context);
        }

        /// <summary>
        ///     Disposes the <see cref="RootContainer"/>.
        /// </summary>
        /// <param name="disposing">
        ///     a value indicating whether managed resources should be disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                // root container already disposed
                return;
            }

            if (disposing)
            {
                // TODO dispose managed resources
            }

            // set disposed flag
            _disposed = true;
        }

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes the root container asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation.</returns>
        public ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                // root container already disposed
                return default;
            }

            // set disposed flag
            _disposed = true;

            return default;
        }
#endif // SUPPORTS_ASYNC_DISPOSABLE
    }
}