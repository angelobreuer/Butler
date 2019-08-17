namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Butler.Registration;

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
        ///     Resolves a single service.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <param name="context">
        ///     the parent resolve context; if <see langword="null"/> a new
        ///     <see cref="ServiceResolveContext"/> is created.
        /// </param>
        /// <returns>the service instance of type <paramref name="serviceType"/></returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object Resolve(Type serviceType, ServiceResolveContext context = null)
        {
            // null-check service type
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType), "Could not resolve service of type <null>.");
            }

            return ResolveInternal(serviceType, context);
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

        private object ResolveInternal(Type serviceType, ServiceResolveContext parentContext = null)
        {
            // create a context for this resolve
            var context = parentContext == null ?
                new ServiceResolveContext(this, this, serviceType) :
                new ServiceResolveContext(parentContext, serviceType);
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