using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    public static class Extension
    {
        public static TAttr GetAttribute<TAttr>(this Type type) 
            where TAttr : Attribute
        {
            return type.GetCustomAttributes(false).Single(a => typeof (TAttr) == a.GetType()) as TAttr;
        }

        public static AutoRegistration IncludeAssembly(this AutoRegistration autoRegistration, string assemblyPath)
        {
            Assembly.LoadFrom(assemblyPath);
            return autoRegistration;
        }

        public static AutoRegistration IncludeAssemblies(this AutoRegistration autoRegistration, IEnumerable<string> assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                autoRegistration.IncludeAssembly(assemblyPath);
            }
            return autoRegistration;
        }

        public static AutoRegistration ConfigureAutoRegistration(this IUnityContainer container)
        {
            return new AutoRegistration(container);
        }
    }
}