using System;

namespace DotJEM.Reflection.Descriptors.Descriptors.References
{
    [Serializable]
    internal class ArrayRef<TDescriptor> where TDescriptor : Descriptor
    {
        private readonly string[] urls;

        public ArrayRef(string[] urls)
        {
            this.urls = urls;
        }

        public static implicit operator ArrayRef<TDescriptor>(string[] urls)
        {
            return new ArrayRef<TDescriptor>(urls);
        }

        public TDescriptor[] Resolve(Descriptor parent)
        {
            return Array.ConvertAll(urls, url =>
            {
                var descriptor = parent.Cache.Get<TDescriptor>(url, parent.LoadInfo);
                descriptor.LoadInfo = parent.LoadInfo;
                return descriptor;
            });
        }
    }
}