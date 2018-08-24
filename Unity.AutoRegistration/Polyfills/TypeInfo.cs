#if NET40

using System;
using System.Collections.Concurrent;
using System.Text;


namespace System.Reflection
{
    public static class TypeExtensions
    {
        private static ConcurrentDictionary<Type, TypeInfo> _cache = new ConcurrentDictionary<Type, TypeInfo>();

        public static TypeInfo GetTypeInfo(this Type type)
        {
            return _cache.GetOrAdd(type, (t) => new TypeInfo(t));
        }
    }

    public class TypeInfo
    {

        private Type _type;
        
        public Assembly Assembly { get { return _type.Assembly; } }

        public Type[] ImplementedInterfaces { get { return _type.GetInterfaces(); } }

        public bool IsGenericType { get { return _type.IsGenericType; } }

        public bool IsGenericTypeDefinition { get { return _type.IsGenericTypeDefinition; } }

        public bool IsInterface { get { return _type.IsInterface; } }
        
        public TypeInfo(Type type)
        {
            _type = type;
        }

        public object[] GetCustomAttributes(bool inherit)
        {
            return _type.GetCustomAttributes(inherit);
        }

        public bool IsAssignableFrom(TypeInfo typeInfo)
        {
            return _type.IsAssignableFrom(typeInfo._type);
        }

    }
    

    public class CustomAttrData
    {

        public Type AttributeType { get; }

        public CustomAttrData(Attribute attribute)
        {
            AttributeType = attribute.GetType();
        }

    }

}

#endif