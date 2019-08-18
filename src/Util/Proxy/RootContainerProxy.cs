#if DEBUG

namespace Butler.Util.Proxy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Butler.Registration;

#if SUPPORTS_LINQ
    using System.Linq;
#endif // SUPPORTS_LINQ

    /// <summary>
    ///     Debugger type proxy for the <see cref="RootContainer"/> type.
    /// </summary>
    internal sealed class RootContainerProxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RootContainerProxy"/> class.
        /// </summary>
        /// <param name="rootContainer">the root container bound to</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="rootContainer"/> is <see langword="null"/>.
        /// </exception>
        public RootContainerProxy(IRootContainer rootContainer)
        {
            RootContainer = rootContainer ?? throw new ArgumentNullException(nameof(rootContainer));
            Settings = new ResolverSettingsProxy(rootContainer);
        }

        /// <summary>
        ///     Gets a proxy holding debugger information of settings for the root container.
        /// </summary>
        public ResolverSettingsProxy Settings { get; }

        /// <summary>
        ///     Gets the container registrations.
        /// </summary>
#if SUPPORTS_READONLY_COLLECTIONS

        public IReadOnlyList<RegistrationProxy> Registrations
#else // SUPPORTS_READONLY_COLLECTIONS
        public RegistrationProxy[] Registrations
#endif // !SUPPORTS_READONLY_COLLECTIONS
        {
            get
            {
                // store container registrations
                var registrations = RootContainer.Registrations;

#if SUPPORTS_LINQ

                // find conflicting types (types which name is not unique under all registrations
                // (e.g. same type name, but in a different namespace).
                var conflictingTypes = registrations.Select(s => s.Key.Name).Where(s => registrations.Count(j => j.Key.Name.Equals(s)) >= 2);

                // build registrations
                return registrations.Select(s => new RegistrationProxy(
                    conflictingTypes.Contains(s.Key.Name) ? s.Key.FullName : s.Key.Name, s.Key, s.Value)).ToArray();
#else // SUPPORTS_LINQ
                // create output list
                var list = new List<RegistrationProxy>();

                // iterate through registrations
                foreach (var pair in registrations)
                {
                    // add to list
                    list.Add(new RegistrationProxy(pair.Key.FullName, pair.Key, pair.Value));
                }

                // create the output array
                return list.ToArray();
#endif // !SUPPORTS_LINQ
            }
        }

        /// <summary>
        ///     Gets the root container bound to.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IRootContainer RootContainer { get; }
    }
}

#endif // DEBUG