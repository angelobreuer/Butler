namespace Butler.Registration
{
    using System;
    using Butler.Lifetime;
    using Butler.Resolver;

    /// <summary>
    ///     Base abstraction of an <see cref="IServiceRegistration"/>.
    /// </summary>
    public abstract class BaseRegistration : IServiceRegistration
    {
        private IServiceLifetime _lifetime = Lifetime.Transient;

        /// <summary>
        ///     Gets the lifetime of the service.
        /// </summary>
        public IServiceLifetime ServiceLifetime
        {
            get => _lifetime;

            set
            {
                if (WasCreated)
                {
                    throw new InvalidOperationException("Can not change the lifetime of the registration " +
                        "after a service was created from the registration.");
                }

                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value),
                        "The specified value can not be null!");
                }

                _lifetime = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the registration created an instance of a service.
        /// </summary>
        protected bool WasCreated { get; private set; }

        /// <summary>
        ///     Registers the service with the <see cref="Lifetime.Scoped"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="BaseRegistration"/> instance</returns>
        public BaseRegistration AsScoped() => WithLifetime(Lifetime.Scoped);

        /// <summary>
        ///     Registers the service with the <see cref="Lifetime.Singleton"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="BaseRegistration"/> instance</returns>
        public BaseRegistration AsSingleton() => WithLifetime(Lifetime.Singleton);

        /// <summary>
        ///     Registers the service with the <see cref="Lifetime.Transient"/> lifetime.
        /// </summary>
        /// <returns>the <see cref="BaseRegistration"/> instance</returns>
        public BaseRegistration AsTransient() => WithLifetime(Lifetime.Transient);

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        public object Create(ServiceResolveContext context)
        {
            WasCreated = true;
            return CreateService(context);
        }

        /// <summary>
        ///     Registers the service with the specified <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="lifetime">the service lifetime</param>
        /// <returns>the <see cref="BaseRegistration"/> instance</returns>
        public BaseRegistration WithLifetime(IServiceLifetime lifetime)
        {
            ServiceLifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            return this;
        }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <returns>the instance</returns>
        protected abstract object CreateService(ServiceResolveContext context);
    }
}