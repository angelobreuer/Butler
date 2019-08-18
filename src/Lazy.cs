#if !SUPPORTS_LAZY

namespace Butler
{
    using System;

#if SUPPORTS_DEBUGGER_CROSS_THREAD_NOTIFY
    using System.Diagnostics;
#endif // SUPPORTS_DEBUGGER_CROSS_THREAD_NOTIFY

    /// <summary>
    ///     Drop-in replacement for the <c>"System.Lazy{T}"</c> class.
    /// </summary>
    /// <typeparam name="T">the type of the object</typeparam>
    public class Lazy<T>
    {
        private readonly LazyValueFactory _valueFactory;
        private Exception _exception;
        private T _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="factory">the lazy value factory</param>
        /// <exception cref="ArgumentNullException">
        ///     thrown if the specified <paramref name="factory"/> is <see langword="null"/>.
        /// </exception>
        public Lazy(LazyValueFactory factory)
            => _valueFactory = factory ?? throw new ArgumentNullException(nameof(factory));

        /// <summary>
        ///     Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="value">the value</param>
        public Lazy(T value)
        {
            IsValueCreated = true;
            _value = value;
        }

        /// <summary>
        ///     Delegate for a lazy-value factory.
        /// </summary>
        /// <returns>the created value</returns>
        public delegate T LazyValueFactory();

        /// <summary>
        ///     Gets a value indicating whether the value was created.
        /// </summary>
        public bool IsValueCreated { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether an exception occurred while creating the value.
        /// </summary>
        /// occurred
        public bool IsValueFaulted => _exception != null;

        /// <summary>
        ///     Gets the value, if it is not created it will be created.
        /// </summary>
        public T Value
        {
            get
            {
                // check if the value is already created
                if (IsValueCreated)
                {
                    // value already created, return it
                    return _value;
                }

                // check if the value was tried to create, but an error occurred
                if (IsValueFaulted)
                {
                    throw _exception;
                }

#if SUPPORTS_DEBUGGER_CROSS_THREAD_NOTIFY
                Debugger.NotifyOfCrossThreadDependency();
#endif // SUPPORTS_DEBUGGER_CROSS_THREAD_NOTIFY

                try
                {
                    // try initialization of the value
                    return _value = _valueFactory();
                }
                catch (Exception ex)
                {
                    // set creation exception and mark as faulted
                    _exception = ex;
                    throw;
                }
            }
        }
    }
}

#endif // !SUPPORTS_LAZY