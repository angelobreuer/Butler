#if DEBUG

// Note this class is only available in the Debug configuration

namespace Butler.Util
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     A class holding the information of a trace element.
    /// </summary>
    [DebuggerDisplay("{Content,nq}", Name = "{Level,nq}")]
    public sealed class TraceElement
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TraceElement"/> class.
        /// </summary>
        /// <param name="level">the level of the trace entry</param>
        /// <param name="content">the content of the trace element</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="content"/> is <see langword="null"/>.
        /// </exception>
        public TraceElement(TraceLevel level, string content)
        {
            Level = level;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        ///     Gets the level of the trace entry.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public TraceLevel Level { get; }

        /// <summary>
        ///     Gets the content of the trace element.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Content { get; }
    }
}

#endif // DEBUG