namespace Butler
{
    using System;
    using System.Runtime.CompilerServices;
    using Butler.Register;
    using Butler.Util;

    /// <summary>
    ///     Base implementation of an <see cref="IServiceResolver"/>.
    /// </summary>
    public abstract class BaseServiceResolver : ServiceRegister, IServiceResolver, IServiceRegister
    {
#if SUPPORTS_SERVICE_PROVIDER
        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        object IServiceProvider.GetService(Type serviceType) => Resolve(serviceType);
#endif

        /// <summary>
        ///     Resolves a service of the specified <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType">the type of the service to resolve</param>
        /// <returns>the resolved service</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="serviceType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        public abstract object Resolve(Type serviceType);

        /// <summary>
        ///     Resolves a service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service to resolve</typeparam>
        /// <returns>the resolved service</returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TService Resolve<TService>() => (TService)Resolve(typeof(TService));

        /// <summary>
        ///     Resolves a lazy-initialized service of the specified <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">the type of the service</typeparam>
        /// <returns>a wrapper that supports lazy-initialization of the specified <typeparamref name="TService"/></returns>
        /// <exception cref="ResolverException">thrown if the service resolve failed.</exception>
        public Lazy<TService> ResolveLazy<TService>()
            => new Lazy<TService>(() => Resolve<TService>());
    }
}