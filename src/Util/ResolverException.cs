namespace Butler.Util
{
    using System;

    /// <summary>
    ///     Exception for resolver failures.
    /// </summary>
    public sealed class ResolverException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        public ResolverException(string message) : base(message)
        {
        }

#if DEBUG

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="traceBuilder">the associated trace builder</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="traceBuilder"/> is <see langword="null"/>.
        /// </exception>
        public ResolverException(string message, TraceBuilder traceBuilder)
            : this(string.Concat(message, "\n\n --- Resolve Trace*\n", traceBuilder.ToString()))
            => TraceBuilder = traceBuilder ?? throw new ArgumentNullException(nameof(traceBuilder));

        /// <summary>
        ///     Gets the associated trace builder.
        /// </summary>
        public TraceBuilder TraceBuilder { get; }

#endif
    }
}