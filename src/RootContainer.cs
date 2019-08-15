namespace Butler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
#if !SUPPORTS_ASYNC_DISPOSABLE

        /// <summary>
        ///     Disposes the root container.
        /// </summary>
        public void Dispose()
        {
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
        ///     Resolves a single service.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the service instance of type <paramref name="serviceType"/></returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        public override object Resolve(Type serviceType)
        {
            // null-check service type
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType), "Could not resolve service of type <null>.");
            }

#if DEBUG
            // create trace builder
            var traceBuilder = new TraceBuilder()
                .AppendResolve(serviceType);
#endif // DEBUG

            // find registration
            var registration = FindRegistration(serviceType);

            // check if the registration failed
            if (registration is null)
            {
                // throw resolver exception
#if DEBUG
                throw new ResolverException($"Could not resolve service of type '{serviceType}' (No registration).", traceBuilder);
#else // DEBUG
                throw new ResolverException($"Could not resolve service of type '{serviceType}' (No registration).");
#endif // !DEBUG
            }

            // TODO TODO TODO
            return registration.Create();
        }

#if SUPPORTS_ASYNC_DISPOSABLE
        /// <summary>
        ///     Disposes the root container asynchronously.
        /// </summary>
        /// <returns>a task that represents the asynchronous operation.</returns>
        public ValueTask DisposeAsync()
        {
            return default;
        }
#endif // SUPPORTS_ASYNC_DISPOSABLE
    }
}