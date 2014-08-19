using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Inspection;

namespace DotJEM.Reflection.Descriptors.Cache
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class DescriptorCache
    {
        private readonly Dictionary<DescriptorUrl, Descriptor> cache = new Dictionary<DescriptorUrl, Descriptor>();

        public T Get<T>(DescriptorUrl url, DescriptorLoadInfo loadInfo) where T : Descriptor
        {
            if (url == null || url.IsEmpty)
                return default(T);

            if (!cache.ContainsKey(url))
            {
                cache[url] = LoadDescriptor(url, loadInfo);
            }
            return (T)cache[url];
        }

        public AssemblyDescriptor LoadAssembly(string path, DescriptorLoadInfo loadInfo)
        {
            DescriptorUrl url = (from key in cache.Keys
                                 where key.AssemblyLocation.Equals(path, StringComparison.InvariantCultureIgnoreCase)
                                 select key).SingleOrDefault() ?? path;
            return Get<AssemblyDescriptor>(url, loadInfo);
        }

        private Descriptor LoadDescriptor(DescriptorUrl url, DescriptorLoadInfo loadInfo)
        {
            //var args = OnDescriptorResolve(new ResolveDescriptorEventArgs(uri));
            Descriptor descriptor;
            if (Context == null)
            {
                using (Context = new AssemblyInspectionContext(loadInfo.WorkingDirectory, loadInfo.ShadowCopy, this))
                {
                   loadInfo.Locations.ForEach(loc => Context.AddDependencyLocation(loc));
                   descriptor = Context.Loader.Load(url);
                   descriptor.LoadInfo = loadInfo;
                   return descriptor;
                }
            }

            descriptor = Context.Loader.Load(url);
            descriptor.LoadInfo = loadInfo;
            descriptor.Cache = this;
            return descriptor;
        }

        //public event EventHandler<ResolveDescriptorEventArgs> DescriptorResolve;

        //private ResolveDescriptorEventArgs OnDescriptorResolve(ResolveDescriptorEventArgs args)
        //{
        //    if (DescriptorResolve != null) 
        //        DescriptorResolve(this, args);
        //    return args;
        //}

        public AssemblyInspectionContext Context { get; set; }
    }

    //internal class ResolveDescriptorEventArgs
    //{
    //}
}