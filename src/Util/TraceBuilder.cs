#if DEBUG

// Note this class is only available in the Debug configuration

namespace Butler.Util
{
    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     An utility class that is useful for building traces when resolving or registering services.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class TraceBuilder
    {
        /// <summary>
        ///     A list holding the trace elements.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IList<TraceElement> _entries;

        /// <summary>
        ///     The lock for the entries.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _entryLock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceBuilder"/> class.
        /// </summary>
        public TraceBuilder()
        {
            _entries = new List<TraceElement>();
            _entryLock = new object();
        }

        public TraceBuilder AppendResolve(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            return Append($"Resolving {serviceType}...");
        }

        public TraceBuilder AppendResolved(Type serviceType, Type implementationType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementationType is null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            return Append($"Resolved {serviceType} as {implementationType}.");
        }

        /// <summary>
        ///     Gets the trace elements.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<TraceElement> Elements
        {
            get
            {
                // acquire lock
                lock (_entryLock)
                {
                    // create copy of the elements
                    return _entries.ToArray();
                }
            }
        }

        /// <summary>
        ///     Gets the debugger display value.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay =>
            // build string with taking care of pluralization (item / items)
            string.Format("Trace View ({0} {1})", _entries.Count, _entries.Count == 1 ? "entry" : "entries");

        /// <summary>
        ///     Appends the specified <paramref name="content"/> to the trace.
        /// </summary>
        /// <param name="content">the trace content</param>
        /// <returns>the <see cref="TraceBuilder"/> instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="content"/> is <see langword="null"/>.
        /// </exception>
        public TraceBuilder Append(string content)
            => Append(TraceLevel.Info, content);

        /// <summary>
        ///     Appends the specified <paramref name="content"/> to the trace.
        /// </summary>
        /// <param name="level">the trace level</param>
        /// <param name="content">the trace content</param>
        /// <returns>the <see cref="TraceBuilder"/> instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="content"/> is <see langword="null"/>.
        /// </exception>
        public TraceBuilder Append(TraceLevel level, string content)
        {
            if (content is null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return Append(new TraceElement(level, content));
        }

        /// <summary>
        ///     Appends the specified <paramref name="element"/> to the trace elements list.
        /// </summary>
        /// <param name="element">the element to add</param>
        /// <returns>the <see cref="TraceBuilder"/> instance</returns>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="element"/> is <see langword="null"/>.
        /// </exception>
        public TraceBuilder Append(TraceElement element)
        {
            // null check element
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // acquire lock
            lock (_entryLock)
            {
                _entries.Add(element);
            }

            return this;
        }

        /// <summary>
        ///     Clears the trace.
        /// </summary>
        public void Clear()
        {
            // acquire lock
            lock (_entryLock)
            {
                // clear all
                _entries.Clear();
            }
        }

        /// <summary>
        ///     Builds a <see cref="string"/> representation of the object.
        /// </summary>
        /// <returns>a <see cref="string"/> representation of the object</returns>
        public override string ToString()
        {
            // the padding for the trace levels
            const int padding = 5;

            // create a string builder
            var stringBuilder = new StringBuilder();

            // acquire lock
            lock (_entryLock)
            {
                // iterate through all entries
                foreach (var element in _entries)
                {
                    // append level name (padded)
                    stringBuilder.Append(element.Level.ToString().PadRight(padding));

                    // append element content (suffixed with a new line)
                    stringBuilder.AppendLine(element.Content);
                }
            }

            // construct string
            return stringBuilder.ToString();
        }
    }
}

#endif // DEBUG