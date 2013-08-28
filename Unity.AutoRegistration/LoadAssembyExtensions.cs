using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Unity.AutoRegistration
{
    public static class LoadAssembyExtensions
    {
        /// <summary>
        /// Loads assembly from given assembly file name.
        /// </summary>
        /// <param name="autoRegistration">Auto registration.</param>
        /// <param name="assemblyPath">Assembly path.</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration LoadAssemblyFrom(this IAutoRegistration autoRegistration, string assemblyPath)
        {
            Assembly.LoadFrom(assemblyPath);
            return autoRegistration;
        }

        /// <summary>
        /// Loads assemblies from given assembly file name.
        /// </summary>
        /// <param name="autoRegistration">Auto registration.</param>
        /// <param name="assemblyPaths">Assembly paths.</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration LoadAssemblyFrom(this IAutoRegistration autoRegistration, IEnumerable<string> assemblyPaths)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                autoRegistration.LoadAssemblyFrom(assemblyPath);
            }
            return autoRegistration;
        }

        /// <summary>
        /// Loads all assemblies found in the assembly path.
        /// </summary>
        /// <param name="autoRegistration">Auto registration.</param>
        /// <param name="assemblyPath">The path containing assemblies to load.</param>
        /// <param name="topLevelOnly">if set to <c>true</c> [top level only].</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration LoadAllAssemblies(this IAutoRegistration autoRegistration, string assemblyPath, bool topLevelOnly = true)
        {
            var searchLevel = topLevelOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories;
            //var filesToLoad = Directory.EnumerateFiles(assemblyPath, "*", searchLevel) // This is more efficient if using >= .NET 4
            var filesToLoad = Directory.GetFiles(assemblyPath, "*", searchLevel)
                .WhereHasDotNetAsseblyExtension()
                .WhereIsDotNetAssembly();

            autoRegistration.LoadAssemblyFrom(filesToLoad);
            return autoRegistration;

        }

        /// <summary>
        /// Loads all assemblies found in the assembly paths.
        /// </summary>
        /// <param name="autoRegistration">The auto registration.</param>
        /// <param name="assemblyPaths">The paths containing assemblies to load.</param>
        /// <param name="topLevelOnly">if set to <c>true</c> [top level only].</param>
        /// <returns>Auto registration</returns>
        public static IAutoRegistration LoadAllAssemblies(this IAutoRegistration autoRegistration, IEnumerable<string> assemblyPaths, bool topLevelOnly = true)
        {
            foreach (var assemblyPath in assemblyPaths)
            {
                autoRegistration.LoadAllAssemblies(assemblyPath, topLevelOnly);
            }

            return autoRegistration;
        }

        internal static IEnumerable<string> WhereHasDotNetAsseblyExtension(this IEnumerable<string> paths)
        {
            return paths.Where(x => x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                || x.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

        }

        internal static IEnumerable<string> WhereIsDotNetAssembly(this IEnumerable<string> paths)
        {
            return paths.Where(x =>
            {
                try
                {
                    AssemblyName.GetAssemblyName(x);
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}