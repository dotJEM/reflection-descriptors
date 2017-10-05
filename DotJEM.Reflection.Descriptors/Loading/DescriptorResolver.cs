using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;
using DotJEM.Reflection.Descriptors.Inspection;

namespace DotJEM.Reflection.Descriptors.Loading
{
    /// <summary>
    /// A Descriptor resolver allows one to load assemblies and types behind the Descriptors.
    /// Note that this will load them into the AppDomain the resolver lives in.
    /// </summary>
    public interface IDescriptorResolver
    {
        Type Resolve(TypeDescriptor self);
        Assembly Resolve(AssemblyDescriptor self);
    }

    public class DescriptorResolver : IDescriptorResolver
    {
        private readonly Dictionary<string, Assembly> cache = 
            new Dictionary<string, Assembly>(); 

        public Type Resolve(TypeDescriptor self)
        {
            return Resolve(self.Assembly).GetType(self.FullName);
        }

        public Assembly Resolve(AssemblyDescriptor self)
        {
            //TODO: Manage unloading.
            if (cache.ContainsKey(self.FullName))
                return cache[self.FullName];

            DependencyResolver.Instance.AddLocation(self.LoadInfo.WorkingDirectory);
            DependencyResolver.Instance.AddLocation(self.LoadInfo.Locations.ToArray());

            DescriptorUrl url = self.Url;
            return url.IsGac ? Assembly.Load(url.AssemblyName) : Assembly.LoadFrom(url.AssemblyLocation);
        }
    }
}
