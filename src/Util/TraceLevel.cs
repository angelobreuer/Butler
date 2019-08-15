#if DEBUG

// Note this class is only available in the Debug configuration

namespace Butler.Util
{
    /// <summary>
    ///     A set of trace levels.
    /// </summary>
    public enum TraceLevel
    {
        /// <summary>
        ///     Denotes that the trace entry is only for information.
        /// </summary>
        Info,

        /// <summary>
        ///     Denotes that for example a resolve failed or a constructor did not match, but the
        ///     resolve can continue.
        /// </summary>
        Warning,

        /// <summary>
        ///     Denotes that an error occurred (e.g. resolve failed, etc.).
        /// </summary>
        Error
    }
}

#endif // DEBUG