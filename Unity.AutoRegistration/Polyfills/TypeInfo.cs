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

        private Collections.Generic.List<CustomAttrData> _customAttributes;

        public Assembly Assembly { get { return _type.Assembly; } }

        public Type[] ImplementedInterfaces { get { return _type.GetInterfaces(); } }

        public bool IsGenericType { get { return _type.IsGenericType; } }

        public bool IsGenericTypeDefinition { get { return _type.IsGenericTypeDefinition; } }

        public bool IsInterface { get { return _type.IsInterface; } }

        public Collections.Generic.IList<CustomAttrData> CustomAttributes
        {
            get
            {
                if (_customAttributes == null)
                {
                    var customAttributes = new Collections.Generic.List<CustomAttrData>();

                    foreach (var attr in _type.GetCustomAttributes(false))
                    {
                        customAttributes.Add(new CustomAttrData((Attribute)attr));
                    }

                    _customAttributes = customAttributes;
                }

                return _customAttributes;
            }
        }

        public TypeInfo(Type type)
        {
            _type = type;
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