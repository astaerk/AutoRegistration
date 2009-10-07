using System;
using System.Linq;
using System.Reflection;

namespace Unity.AutoRegistration
{
    public static class If
    {
        public static bool DecoratedWith<TAttr>(this Type type)
            where TAttr : Attribute
        {
            return type.GetCustomAttributes(false).Any(a => a.GetType() == typeof(TAttr));
        }

        public static bool Implements<TContract>(this Type type) where TContract : class
        {
            return type.GetInterfaces().Any(i => i == typeof(TContract));
        }

        public static bool ImplementsITypeName(this Type type)
        {
            return type.GetInterfaces().Any(i => i.Name.TrimStart('I') == type.Name);
        }

        public static bool ImplementsSingleInterface(this Type type)
        {
            return type.GetInterfaces().Count() == 1;
        }

        public static bool AnyType(this Type type)
        {
            return true;
        }

        public static bool TypeIs<T>(this Type type)
        {
            return type == typeof(T);
        }

        public static bool AnyAssembly(this Assembly assembly)
        {
            return true;
        }

        public static bool ContainsType<T>(this Assembly assembly)
        {
            return typeof (T).Assembly == assembly;
        }
    }
}