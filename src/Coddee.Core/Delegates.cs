namespace Coddee
{
    /// <summary>
    /// Project an item into another type.
    /// </summary>
    /// <typeparam name="TTarget">The targeted type.</typeparam>
    /// <typeparam name="TSource">The type of the original item.</typeparam>
    /// <param name="sourceItem">The item to be projected</param>
    public delegate TTarget Projection<in TSource, out TTarget>(TSource sourceItem);

    /// <summary>
    /// Find the similar item in a different type.
    /// </summary>
    /// <typeparam name="TTarget">The targeted type.</typeparam>
    /// <typeparam name="TSource">The type of the original item.</typeparam>
    /// <param name="sourceItem">The item to be found</param>
    public delegate TTarget ItemLocator<in TSource, out TTarget>(TSource sourceItem);
}
