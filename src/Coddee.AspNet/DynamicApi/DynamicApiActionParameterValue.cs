namespace Coddee.AspNet
{
    /// <summary>
    /// <see cref="IDynamicApiAction"/> parameters values.
    /// </summary>
    public class DynamicApiActionParameterValue
    {
        /// <summary>
        /// The parameter object.
        /// </summary>
        public DynamicApiActionParameter Parameter { get; set; }

        /// <summary>
        /// The parameter value.
        /// </summary>
        public object Value { get; set; }

    }
}