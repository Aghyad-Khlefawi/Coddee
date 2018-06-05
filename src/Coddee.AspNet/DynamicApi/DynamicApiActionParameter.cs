using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coddee.AspNet
{
    /// <summary>
    /// <see cref="IDynamicApiAction"/> parameters.
    /// </summary>
    public class DynamicApiActionParameter
    {
        /// <inheritdoc />
        public DynamicApiActionParameter(ParameterInfo param)
        {
            Name = param.Name.ToLower();
            Type = param.ParameterType;
            Index = param.Position;
        }

        /// <summary>
        /// Parameter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameter type.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The index of the parameter.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Returns the parameters of a specific method.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static List<DynamicApiActionParameter> GetParameters(MethodInfo method)
        {
            return method.GetParameters().Select(e => new DynamicApiActionParameter(e)).ToList();
        }
    }
}