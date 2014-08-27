using System;

namespace DotJEM.Reflection.Descriptors.Descriptors.References
{
    [Serializable]
    internal class ObjectRef<TDescriptor> where TDescriptor : Descriptor
    {
        private readonly string url;

        public ObjectRef(string url)
        {
            this.url = url;
        }

        public static implicit operator ObjectRef<TDescriptor>(string url)
        {
            return new ObjectRef<TDescriptor>(url);
        }

        public TDescriptor Resolve(Descriptor parent)
        {
            var descriptor = parent.Cache.Get<TDescriptor>(url, parent.LoadInfo);
            descriptor.LoadInfo = parent.LoadInfo;
            return descriptor;
        }
    }
}