#if DEBUG

namespace Butler.Util.Proxy
{
    using System;
    using System.Diagnostics;
    using Butler.Registration;

    /// <summary>
    ///     An union of a service registration alias ( <see cref="Name"/>), a service type (
    ///     <see cref="ServiceType"/>) and a service registration ( <see cref="Registration"/>) used
    ///     for advanced debugging views.
    /// </summary>
    [DebuggerDisplay("{Registration.GetType().Name,nq}", Name = "{Name,nq}")]
    internal class RegistrationProxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RegistrationProxy"/> class.
        /// </summary>
        /// <param name="name">the name / alias of the registration</param>
        /// <param name="serviceType">the type of the service</param>
        /// <param name="registration">the registration</param>
        public RegistrationProxy(string name, Type serviceType, IServiceRegistration registration)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
        }

        /// <summary>
        ///     Gets the key.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Name { get; }

        /// <summary>
        ///     Gets the associated value.
        /// </summary>
        public IServiceRegistration Registration { get; }

        /// <summary>
        ///     Gets the item.
        /// </summary>
        public Type ServiceType { get; }
    }
}

#endif // DEBUG