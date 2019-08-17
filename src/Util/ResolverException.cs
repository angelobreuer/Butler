namespace Butler.Util
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     Exception for resolver failures.
    /// </summary>
    public sealed class ResolverException : Exception
    {
        /// <summary>
        ///     The message used when no message is set explicitly.
        /// </summary>
        private const string UnknownErrorMessage = "Unknown error.";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        public ResolverException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="context">the current resolver context</param>
        public ResolverException(string message, ServiceResolveContext context)
            : this(BuildMessage(message, context))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        public ResolverException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="context">the current resolver context</param>
        public ResolverException(ServiceResolveContext context)
            : this(BuildMessage(UnknownErrorMessage, context))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="innerException">the inner nested exception</param>
        public ResolverException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="context">the current resolver context</param>
        /// <param name="innerException">the inner nested exception</param>
        public ResolverException(string message, ServiceResolveContext context, Exception innerException)
            : base(BuildMessage(message, context), innerException)
        {
        }

        /// <summary>
        ///     Gets the associated resolve context (may be <see langword="null"/>).
        /// </summary>
        public ServiceResolveContext Context { get; }

        /// <summary>
        ///     Builds the message for the specified <paramref name="context"/>.
        /// </summary>
        /// <remarks>All parameters are optional.</remarks>
        /// <param name="message">the base message</param>
        /// <param name="context">the current resolver context</param>
        /// <returns>the message build</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string BuildMessage(string message = null, ServiceResolveContext context = null)
        {
            // use <Unknown Error> if no message is set
            if (string.IsNullOrWhiteSpace(message))
            {
                message = UnknownErrorMessage;
            }

#if DEBUG
            // check if the context is not available
            if (context is null)
            {
                // context is null, simply return message
                return message;
            }

            return string.Concat(message, "\n\n --- Resolve Trace\n", context.TraceBuilder);
#else // DEBUG
            return message;
#endif // !DEBUG
        }
    }
}