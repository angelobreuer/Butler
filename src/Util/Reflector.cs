#if !NO_REFLECTION

namespace Butler.Util
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Butler.Register;
    using Butler.Resolver;
    using System.Linq;

    /// <summary>
    ///     An utility class for resolving a service with dependencies from an <see cref="IServiceResolver"/>.
    /// </summary>
    public static class Reflector
    {
        /// <summary>
        ///     Gets a value indicating whether the specified <paramref name="parameters"/> can be
        ///     resolved using the specified <paramref name="register"/>.
        /// </summary>
        /// <param name="register">the register to use</param>
        /// <param name="parameters">the service implementation type constructor parameters</param>
        /// <returns>
        ///     a value indicating whether the specified <paramref name="parameters"/> can be
        ///     resolved using the specified <paramref name="register"/>
        /// </returns>
        public static bool CanUseConstructor(IServiceRegister register, IEnumerable<ParameterInfo> parameters)
            => parameters.All(s => register.FindRegistration(s.ParameterType) != null);

        /// <summary>
        ///     Creates a new instance of the specified service type (in <paramref name="context"/>).
        /// </summary>
        /// <typeparam name="TImplementation">the type of the implementation to construct</typeparam>
        /// <param name="context">the resolve context</param>
        /// <returns>the service instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        public static TImplementation Resolve<TImplementation>(ServiceResolveContext context)
            => (TImplementation)Resolve(typeof(TImplementation), context);

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
            var constructor = ResolveConstructor(context, availableConstructors, implementationType);

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

        /// <summary>
        ///     Resolves the matching constructor for the specified
        ///     <paramref name="implementationType"/> with respecting the specified <paramref name="constructionMode"/>.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        /// <param name="availableConstructors">
        ///     an enumerable that enumerates through the available constructors for the specified <paramref name="implementationType"/>.
        /// </param>
        /// <param name="implementationType">
        ///     the type of the service implementation to resolve the constructor for
        /// </param>
        /// <param name="constructionMode">the service construction mode.</param>
        /// <returns>the constructor chosen fro the specified <paramref name="implementationType"/></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     thrown if the specified <paramref name="constructionMode"/> is unsupported or not
        ///     defined in the <see cref="ServiceConstructionMode"/> enumeration.
        /// </exception>
        /// <exception cref="ResolverException">
        ///     thrown if no passable constructor was found for the specified <paramref name="implementationType"/>
        /// </exception>
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
            if (context.ConstructionMode == ServiceConstructionMode.Mixed)
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
            if (context.ConstructionMode == ServiceConstructionMode.PreferComplexConstructor
                || context.ConstructionMode == ServiceConstructionMode.Mixed)
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
                    throw new ResolverException($"Could not find a passable constructor for '{implementationType}'.", context);
                }

                return constructor.Key;
            }

            // search parameter-less constructor
            if (context.ConstructionMode == ServiceConstructionMode.PreferParameterlessConstructor)
            {
                // iterate through all available constructors and find a parameter-less
                var constructor = availableConstructors.FirstOrDefault(s => s.GetParameters().Length == 0);

                // check if no constructor was found
                if (constructor is null)
                {
                    // throw exception: No parameter-less constructor
                    throw new ResolverException($"Could not find a parameter-less constructor for '{implementationType}'.", context);
                }

                return constructor;
            }

            // throw exception: Service Construction mode out of range
            throw new ArgumentOutOfRangeException(nameof(constructionMode), constructionMode,
                $"Invalid or unsupported service construction mode: '{constructionMode}'.");
        }
    }
}

#endif // !NO_REFLECTION