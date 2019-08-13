namespace Butler.Register
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Butler.Registration;

    /// <summary>
    ///     Abstraction for a service register.
    /// </summary>
    public abstract class BaseServiceRegister : IServiceRegister
    {
        /// <summary>
        ///     Gets a value indicating whether new service registrations are not allowed.
        /// </summary>
        public abstract bool IsReadOnly { get; }

        /// <summary>
        ///     Gets all service registrations in the register.
        /// </summary>
        public abstract IReadOnlyList<KeyValuePair<Type, IServiceRegistration>> Registrations { get; }

        /// <summary>
        ///     Creates a read-only instance of the service register.
        /// </summary>
        /// <returns>the read-only service register</returns>
        public ReadOnlyServiceRegister AsReadOnly()
            => new ReadOnlyServiceRegister(Registrations);

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
        public abstract void Register(Type type, IServiceRegistration registration);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Register<T>(IServiceRegistration registration)
            => Register(typeof(T), registration);

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="T">the type of the registration</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register<T>(T instance) where T : class
            => Register<T>(new InstanceRegistration(instance));

        /// <summary>
        ///     Registers the specified <paramref name="instance"/> as a singleton.
        /// </summary>
        /// <typeparam name="TAbstraction">the type of the registration</typeparam>
        /// <typeparam name="TImplementation">the type of the implementation</typeparam>
        /// <param name="instance">the instance</param>
        /// <exception cref="InvalidOperationException">
        ///     thrown if the register is read-only ( <see cref="IsReadOnly"/>).
        /// </exception>
        public void Register<TAbstraction, TImplementation>(TImplementation instance)
            where TImplementation : class, TAbstraction
            => Register<TAbstraction>(new InstanceRegistration(instance));
    }
}