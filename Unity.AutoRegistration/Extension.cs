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

        public static AutoRegistration LoadAssemblyFrom(this AutoRegistration autoRegistration, string assemblyPath)
        {
            Assembly.LoadFrom(assemblyPath);
            return autoRegistration;
        }

        public static AutoRegistration LoadAssembliesFrom(this AutoRegistration autoRegistration, IEnumerable<string> assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                autoRegistration.LoadAssemblyFrom(assemblyPath);
            }
            return autoRegistration;
        }

        public static AutoRegistration ConfigureAutoRegistration(this IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            return new AutoRegistration(container);
        }

        public static AutoRegistration ExcludeSystemAssemblies(this AutoRegistration autoRegistration)
        {
            autoRegistration.ExcludeAssemblies(a => a.GetName().FullName.StartsWith("System") 
                || a.GetName().FullName.StartsWith("mscorlib"));
            return autoRegistration;
        }
    }
}