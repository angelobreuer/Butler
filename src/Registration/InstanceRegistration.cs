namespace Butler.Registration
{
    using System;

    /// <summary>
    ///     Registration of a static instance.
    /// </summary>
    public sealed class InstanceRegistration : IServiceRegistration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InstanceRegistration"/> class.
        /// </summary>
        /// <param name="instance">the static instance</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        public InstanceRegistration(object instance)
            => Instance = instance ?? throw new ArgumentNullException(nameof(instance));

        /// <summary>
        ///     Gets the static instance.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <returns>the instance</returns>
        public object Create() => Instance;
    }
}