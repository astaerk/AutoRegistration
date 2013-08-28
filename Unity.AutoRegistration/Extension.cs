using System;
using System.Linq;
using Microsoft.Practices.Unity;

namespace Unity.AutoRegistration
{
    /// <summary>
    /// Extension methods to various types
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Gets type attribute.
        /// </summary>
        /// <typeparam name="TAttr">Type of the attribute.</typeparam>
        /// <param name="type">Target type.</param>
        /// <returns>Attribute value</returns>
        public static TAttr GetAttribute<TAttr>(this Type type) 
            where TAttr : Attribute
        {
            return type.GetCustomAttributes(false).Single(a => typeof (TAttr) == a.GetType()) as TAttr;
        }

        /// <summary>
        /// Configures auto registration - starts chain of fluent configuration
        /// </summary>
        /// <param name="container">Unity container.</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration ConfigureAutoRegistration(this IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            return new AutoRegistration(container);
        }

        /// <summary>
        /// Adds rule to exclude certain assemblies (that name starts with System or mscorlib) 
        /// and not consider their types
        /// </summary>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration ExcludeSystemAssemblies(this IAutoRegistration autoRegistration)
        {
            autoRegistration.ExcludeAssemblies(a => a.GetName().FullName.StartsWith("System.") 
                || a.GetName().FullName.StartsWith("mscorlib")
                || a.GetName().Name.Equals("System"));
            return autoRegistration;
        }
    }
}