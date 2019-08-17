namespace Butler
{
    using System;

    /// <summary>
    ///     An attribute that controls the selector choice of a service resolver. When applied to a
    ///     constructor the constructor is preferred to be used for service resolve.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class PreferConstructorAttribute : Attribute
    {
    }
}