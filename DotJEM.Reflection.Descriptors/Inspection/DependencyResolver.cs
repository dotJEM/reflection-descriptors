using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    public class DependencyResolverProxy : MarshalByRefObject
    {
        private readonly DependencyResolver resolver = DependencyResolver.Instance;

        public bool AddLocation(string location)
        {
            return resolver.AddLocation(location);
        }

        public bool RemoveLocation(string location)
        {
            return resolver.RemoveLocation(location);
        }
    }

    public sealed class DependencyResolver 
    {
        public static DependencyResolver Instance { get; } = new DependencyResolver();

        private readonly HashSet<string> locations = new HashSet<string>();

        private DependencyResolver()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolve;
        }

        public bool AddLocation(params string[] dirs)
        {
            return dirs.Aggregate(true, (current, dir) => locations.Add(dir) && current);
        }

        public bool RemoveLocation(string location)
        {
            return locations.Remove(location);
        }

        private Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var resolved = locations
                .Select(location => LoadFromLocation(args, location))
                .FirstOrDefault(assembly => assembly != null);
            return resolved;
        }

        private static Assembly LoadFromLocation(ResolveEventArgs args, string location)
        {
            return LoadAs(location, args, "exe") ?? LoadAs(location, args, "dll");
        }

        private static Assembly LoadAs(string location, ResolveEventArgs args, string extention)
        {
            string path = Path.Combine(location, $"{new AssemblyName(args.Name).Name}.{extention}");
            return File.Exists(path) ? Assembly.LoadFile(path) : null;
        }
    }
}