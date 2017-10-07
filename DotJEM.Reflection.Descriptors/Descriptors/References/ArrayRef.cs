using System;

namespace DotJEM.Reflection.Descriptors.Descriptors.References
{
    [Serializable]
    internal class ArrayRef<TDescriptor> where TDescriptor : Descriptor
    {
        private readonly string[] uris;

        public ArrayRef(string[] uris)
        {
            this.uris = uris;
        }

        public static implicit operator ArrayRef<TDescriptor>(string[] urls)
        {
            return new ArrayRef<TDescriptor>(urls);
        }

        public TDescriptor[] Resolve(Descriptor parent)
        {
            return Array.ConvertAll(uris, uri =>
            {
                var descriptor = parent.Cache.Get<TDescriptor>(uri, parent.LoadInfo);
                descriptor.LoadInfo = parent.LoadInfo;
                return descriptor;
            });
        }
    }
}