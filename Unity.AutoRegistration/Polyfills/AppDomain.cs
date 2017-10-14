#if NETSTANDARD1_6

using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.AutoRegistration.Polyfills
{
    public class AppDomain
    {

        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                try
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
                catch (System.IO.FileNotFoundException)
                { }
            }
            return assemblies.ToArray();
        }
        
    }
}

#endif