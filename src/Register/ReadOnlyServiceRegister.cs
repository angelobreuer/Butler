namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using Butler.Registration;

    /// <summary>
    ///     A class that provides a read-only service register.
    /// </summary>
    public sealed class ReadOnlyServiceRegister : IServiceRegister
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyServiceRegister"/> class.
        /// </summary>
        /// <param name="registrations">the static registrations</param>
        public ReadOnlyServiceRegister(IReadOnlyList<IServiceRegistration> registrations)
            => Registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));

        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public bool IsReadOnly { get; } = true;

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        public IReadOnlyList<IServiceRegistration> Registrations { get; }

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register(IServiceRegistration registration)
            => throw new InvalidOperationException("The register is read-only.");
    }
}