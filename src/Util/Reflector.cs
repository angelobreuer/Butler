namespace Butler.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Butler.Register;

    /// <summary>
    ///     An utility class for resolving a service with dependencies from an <see cref="IServiceResolver"/>.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        ///     Creates a new instance of the specified service type (in <paramref name="context"/>).
        /// </summary>
        /// <typeparam name="TImplementation">the type of the implementation to construct</typeparam>
        /// <param name="context">the resolve context</param>
        /// <returns>the service instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TImplementation Resolve<TImplementation>(ServiceResolveContext context)
            => (TImplementation)Resolve(typeof(TImplementation), context);

        public static ConstructorInfo ResolveConstructor(ServiceResolveContext context,
            IEnumerable<ConstructorInfo> availableConstructors, Type implementationType,
            ServiceConstructionMode constructionMode = ServiceConstructionMode.Mixed)
        {
            // check if no constructors are available
            if (!availableConstructors.Any())
            {
                return null;
            }

            // check whether the mode is mixed
            if (constructionMode == ServiceConstructionMode.Mixed)
            {
                // iterate through all constructors
                foreach (var constructor in availableConstructors)
                {
                    // check if the constructor is preferred
                    if (constructor.GetCustomAttribute<PreferConstructorAttribute>() != null)
                    {
                        // constructor has the attribute
                        return constructor;
                    }
                }
            }

            // search complexest constructor
            if (constructionMode == ServiceConstructionMode.PreferComplexConstructor || constructionMode == ServiceConstructionMode.Mixed)
            {
                // iterate through all available constructors and find the complexest
                var constructor = availableConstructors
                    .Select(s => new KeyValuePair<ConstructorInfo, IReadOnlyList<ParameterInfo>>(s, s.GetParameters()))
                    .OrderByDescending(s => s.Value.Count) // order by parameter count (descending)
                    .FirstOrDefault(s => CanUseConstructor(context.Register, s.Value));

                // check if no constructor was found
                if (constructor.Key is null)
                {
                    // throw exception: No constructor was found for the type.
                    throw new ResolverException($"Could not find a passable constructor for '{implementationType}'."); // TODO pass trace builder
                }

                return constructor.Key;
            }

            // search parameter-less constructor
            if (constructionMode == ServiceConstructionMode.PreferParameterlessConstructor)
            {
                // iterate through all available constructors and find a parameter-less
                var constructor = availableConstructors.FirstOrDefault(s => s.GetParameters().Length == 0);

                // check if no constructor was found
                if (constructor is null)
                {
                    // throw exception: No parameter-less constructor
                    throw new ResolverException($"Could not find a parameter-less constructor for '{implementationType}'."); // TODO pass trace builder
                }

                return constructor;
            }

            // throw exception: Service Construction mode out of range
            throw new ArgumentOutOfRangeException(nameof(constructionMode), constructionMode,
                $"Invalid or unsupported service construction mode: '{constructionMode}'.");
        }

        public static bool CanUseConstructor(IServiceRegister register, IEnumerable<ParameterInfo> parameters)
            => parameters.All(s => register.FindRegistration(s.ParameterType) != null);

        /// <summary>
        ///     Creates a new instance of the specified service type (in <paramref name="context"/>).
        /// </summary>
        /// <param name="implementationType">the type of the implementation to construct</param>
        /// <param name="context">the resolve context</param>
        /// <returns>the service instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="implementationType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        public static object Resolve(Type implementationType, ServiceResolveContext context)
        {
            // TODO add parameter for service construction mode

            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // resolve constructors
#if SUPPORTS_REFLECTION
            var availableConstructors = implementationType.GetConstructors();
#else // SUPPORTS_REFLECTION
            var availableConstructors = implementationType.GetTypeInfo().DeclaredConstructors;
#endif // !SUPPORTS_REFLECTION

            // resolve constructor for the type
            var constructor = ResolveConstructor(context, availableConstructors, implementationType, ServiceConstructionMode.Mixed);

#if DEBUG
            // trace output
            context.TraceBuilder.AppendConstructorResolve(implementationType, constructor);
#endif // DEBUG

            // get constructor parameters and create array holding the parameters for the constructor
            var constructorParameters = constructor.GetParameters();
            var invocationParameters = new object[constructorParameters.Length];

            // resolve services for the constructor
            for (var index = 0; index < constructorParameters.Length; index++)
            {
                // the type of the service being resolved
                var serviceType = constructorParameters[index].ParameterType;

                // resolve service and add to constructor parameter invocation list
                invocationParameters[index] = context.Resolver.Resolve(serviceType, context);
            }

            // invoke constructor and constructor type
            return constructor.Invoke(invocationParameters);
        }
    }
}