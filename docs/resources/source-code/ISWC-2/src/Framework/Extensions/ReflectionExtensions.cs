using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SpanishPoint.Azure.Iswc.Framework.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ReflectionExtensions
    {
        private static IEnumerable<Assembly> LoadedAssemblies = new List<Assembly>();
        public static IEnumerable<Assembly> GetAllLoadedAssemblies(this AppDomain appDomain, string prefix = "", string? baseDirectory = null)
        {
            if (LoadedAssemblies.Any()) return LoadedAssemblies;

            prefix = !string.IsNullOrEmpty(prefix) ? prefix : AssemblyPrefix;
            string[] assemblyMatch = new[] { $"{ prefix }*.dll" };

            baseDirectory = !string.IsNullOrWhiteSpace(baseDirectory) ? baseDirectory : appDomain.BaseDirectory;

            LoadedAssemblies = Directory.EnumerateFiles(baseDirectory, "*.dll", SearchOption.AllDirectories)
                      .Where(filename => assemblyMatch.Any(pattern => Regex.IsMatch(filename, pattern)))
                      .Select(Assembly.LoadFrom);

            return LoadedAssemblies;
        }

        private static readonly string AssemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";
        private static string AssemblyPrefix => AssemblyName.Substring(0, AssemblyName.LastIndexOf('.')) + '.';

        public static IEnumerable<T> GetComponentsOfType<T>(this AppDomain appDomain, IServiceProvider serviceProvider)
        {
            foreach (var type in appDomain.GetAllLoadedAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => typeof(T).IsAssignableFrom(x) && x.IsClass && !x.IsInterface))
            {
                yield return (T)serviceProvider.GetService(type)!;
            }
        }
    }
}
