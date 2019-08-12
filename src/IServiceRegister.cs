namespace Butler
{
    using System;
    using System.Collections.Generic;
    using Butler.Registration;

    /// <summary>
    ///     Interface for a service register.
    /// </summary>
    public interface IServiceRegister
    {
        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        IReadOnlyList<IServiceRegistration> Registrations { get; }

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        void Register(IServiceRegistration registration);
    }
}