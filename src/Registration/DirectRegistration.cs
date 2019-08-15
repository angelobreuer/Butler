﻿namespace Butler.Registration
{
    /// <summary>
    ///     Provides an <see cref="IServiceRegistration"/> for direct parameterless constructors.
    /// </summary>
    /// <typeparam name="TImplementation">the service implementation</typeparam>
    public class DirectRegistration<TImplementation>
        : IServiceRegistration where TImplementation : new()
    {
        /// <summary>
        ///     Creates the instance.
        /// </summary>
        /// <param name="resolver">the calling resolver</param>
        /// <returns>the instance</returns>
        public object Create(IServiceResolver resolver) => new TImplementation();
    }
}