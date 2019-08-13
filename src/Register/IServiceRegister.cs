namespace Butler.Register
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
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        IReadOnlyList<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        ReadOnlyServiceRegister AsReadOnly();

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <param name="type">the type of the registration</param>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="type"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        void Register(Type type, IServiceRegistration registration);

        /// <summary>
        ///     Registers the specified <paramref name="registration"/>.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="registration">the registration to register</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="registration"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        void Register<T>(IServiceRegistration registration);

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        void Register<T>(T instance) where T : class;

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TAbstraction">the type of the registration</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        void Register<TAbstraction, TImplementation>(TImplementation instance)
            where TImplementation : class, TAbstraction;
    }
}