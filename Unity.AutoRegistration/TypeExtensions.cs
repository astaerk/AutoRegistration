using System;
using System.Linq;
using System.Reflection;

namespace Unity.AutoRegistration
{
    public static class TypeExtensions
    {
        public static Type[] GetCorrectInterfaces(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition)
                //typeInfo.ImplementedInterfaces.First().DeclaringType
                return typeInfo.ImplementedInterfaces.Select(t => t.GetTypeInfo().Assembly.GetType(t.Namespace + "." + t.Name)).ToArray();
            else
                return typeInfo.ImplementedInterfaces.ToArray();
        }
    }
}
