namespace Butler
{
    /// <summary>
    ///     A set of supported container service resolution mode.
    /// </summary>
    public enum ContainerResolveMode
    {
        /// <summary>
        ///     Denotes that it first should be tried to resolve the service from the parent
        ///     container, then from the container itself.
        /// </summary>
        ParentFirst,

        /// <summary>
        ///     Denotes that it first should be tried to resolve the service from the container
        ///     itself, then from the parent.
        /// </summary>
        ChildFirst
    }
}