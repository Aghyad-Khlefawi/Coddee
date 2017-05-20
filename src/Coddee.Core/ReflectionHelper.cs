using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coddee
{
    public static class ReflectionHelper
    {
        public static IEnumerable<PropertyInfo> GetProperties(
            this TypeInfo type,
            Func<PropertyInfo, bool> predicate = null)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            properties.AddRange(predicate == null ? type.DeclaredProperties : type.DeclaredProperties.Where(predicate));
            if (type.BaseType != null && type.BaseType.Name != typeof(object).Name)
                properties.AddRange(GetProperties(type.BaseType.GetTypeInfo()));
            return properties;
        }

        public static PropertyInfo GetProperty(
            this TypeInfo type,
            string name)
        {
            foreach (var property in type.DeclaredProperties)
            {
                if (property.Name == name)
                    return property;
            }
            if (type.BaseType != null && type.BaseType.Name != typeof(object).Name)
                return type.BaseType.GetTypeInfo().GetProperty(name);
            return null;
        }
    }
}