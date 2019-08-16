namespace Butler
{
    /// <summary>
    ///     Delegate for service factories.
    /// </summary>
    /// <typeparam name="T">the type of the implementation the factory provides</typeparam>
    /// <param name="resolver">the current resolver context</param>
    /// <returns>the service instance</returns>
    public delegate T ServiceFactory<out T>(ServiceResolveContext resolver);
}