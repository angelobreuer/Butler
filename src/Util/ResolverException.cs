namespace Butler.Util
{
    using System;
    using System.Runtime.CompilerServices;
    using Butler.Resolver;

#if SUPPORTS_SERIALIZABLE
    using System.Runtime.Serialization;
#endif // SUPPORTS_SERIALIZABLE

    /// <summary>
    ///     Exception for resolver failures.
    /// </summary>
#if SUPPORTS_SERIALIZABLE
    [Serializable]
#endif // SUPPORTS_SERIALIZABLE

    public class ResolverException : Exception
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
            : this(BuildMessage(message, context)) => Context = context;

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
            => Context = context;

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
            : base(BuildMessage(message, context), innerException) => Context = context;

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
#if SUPPORTS_COMPILER_SERVICES

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#else // SUPPORTS_COMPILER_SERVICES
        [MethodImpl(256 /* Aggressive Inlining */)]
#endif // !SUPPORTS_COMPILER_SERVICES
        private static string BuildMessage(string message = null, ServiceResolveContext context = null)
        {
            // use <Unknown Error> if no message is set
#if SUPPORTS_WHITESPACE_CHECK
            if (string.IsNullOrWhiteSpace(message))
#else // !SUPPORTS_WHITESPACE_CHECK
            if (string.IsNullOrEmpty(message))
#endif // SUPPORTS_WHITESPACE_CHECK
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

#if SUPPORTS_SERIALIZABLE
        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolverException"/> class.
        /// </summary>
        /// <param name="serializationInfo">the serialization information</param>
        /// <param name="streamingContext">the streaming context</param>
        protected ResolverException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
#endif // SUPPORTS_SERIALIZABLE
    }
}