using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;
using DotJEM.Reflection.Descriptors.Inspection;

namespace DotJEM.Reflection.Descriptors.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DescriptorCache
    {
        //private readonly Dictionary<DescriptorUri, Descriptor> cache = new Dictionary<DescriptorUri, Descriptor>();

        private readonly ConcurrentDictionary<DescriptorType, ConcurrentDictionary<DescriptorUri, Descriptor>> cache 
            = new ConcurrentDictionary<DescriptorType, ConcurrentDictionary<DescriptorUri, Descriptor>>();

        public DescriptorCache()
        {
            cache.TryAdd(DescriptorType.Assembly, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Type, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Field, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Property, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Constructor, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Method, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Event, new ConcurrentDictionary<DescriptorUri, Descriptor>());
            cache.TryAdd(DescriptorType.Attribute, new ConcurrentDictionary<DescriptorUri, Descriptor>());
        }

        public T Get<T>(DescriptorUri uri, LoadInfo loadInfo) where T : Descriptor
        {
            if (uri == null || !uri.IsValid)// || url.IsEmpty)
                return default(T);

            return (T) cache[uri.Type].GetOrAdd(uri, key => LoadDescriptor(key, loadInfo));

            //if (!cache.ContainsKey(uri))
            //{
            //    var descriptor = LoadDescriptor(url, loadInfo);
            //    return (T) (cache[descriptor.Url] = descriptor);
            //}
            //return (T)cache[url];
        }

        public AssemblyDescriptor LoadAssembly(string path, LoadInfo loadInfo)
        {
            path = Path.GetFullPath(path);
            DescriptorUri url = (from key in cache[DescriptorType.Assembly].Keys
                                 where key.AssemblyPath.Equals(path, StringComparison.InvariantCultureIgnoreCase)
                                 select key).SingleOrDefault() ?? new AssemblyDescriptorUri($"assembly://{path}", path);
            return Get<AssemblyDescriptor>(url, loadInfo);
        }

        private Descriptor LoadDescriptor(DescriptorUri uri, LoadInfo loadInfo)
        {
            Descriptor descriptor;
            if (Context == null)
            {
                using (Context = new AssemblyInspectionContext(loadInfo.WorkingDirectory, loadInfo.ShadowCopy, this))
                {
                   loadInfo.Locations.ForEach(loc => Context.AddDependencyLocation(loc));
                   descriptor = Context.Loader.Load(uri.Value);
                   descriptor.LoadInfo = loadInfo;
                   return descriptor;
                }
            }
            object value = Context.Loader.Load(uri.Value);
            descriptor = (Descriptor) value;
            descriptor.LoadInfo = loadInfo;
            descriptor.Cache = this;
            return descriptor;
        }

        public AssemblyInspectionContext Context { get; set; }
    }

}