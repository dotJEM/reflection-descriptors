using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotJEM.Reflection.Descriptors.Cache;

namespace DotJEM.Reflection.Descriptors.Descriptors
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
            return Array.ConvertAll(urls, url => parent.Cache.Get<TDescriptor>(url, parent.LoadInfo));
        }
    }

    [Serializable]
    internal class ObjectRef<TDescriptor> where TDescriptor : Descriptor
    {
    }

    [Serializable]
    public abstract class Descriptor
    {
        public DescriptorUrl Url { get; private set; }
        public abstract DescriptorType DescriptorType { get; }

        internal DescriptorLoadInfo LoadInfo { get; set; }
        internal DescriptorCache Cache { get; set; }

        internal protected Descriptor(string url)
        {
            Url = url;
        }
    }

    [Serializable]
    public class AssemblyDescriptor : Descriptor
    {
        public bool IsDynamic { get; private set; }
        public bool IsFullyTrusted { get; private set; }
        public bool GlobalAssemblyCache { get; private set; }

        public string FullName { get; private set; }
        public string Location { get; private set; }
        public string CodeBase { get; private set; }

        public override DescriptorType DescriptorType { get { return DescriptorType.Assembly; } }

        private readonly ArrayRef<TypeDescriptor> types;
        private readonly ArrayRef<AssemblyDescriptor> assemblies;

        public TypeDescriptor[] Types { get { return types.Resolve(this); } }
        public AssemblyDescriptor[] ReferencedAssemblies { get { return assemblies.Resolve(this); } }

        internal AssemblyDescriptor(Assembly assembly)
            : base(assembly.CreateReference())
        {
            IsDynamic = assembly.IsDynamic;
            IsFullyTrusted = assembly.IsFullyTrusted;
            GlobalAssemblyCache = assembly.GlobalAssemblyCache;

            FullName = assembly.FullName;
            Location = assembly.Location;
            CodeBase = assembly.CodeBase;

            types = (from type in assembly.GetTypes() select type.CreateReference()).ToArray();
            assemblies = (from assem in assembly.GetReferencedAssemblies() select assem.CreateReference()).ToArray();

            //attributes = (from assem in assembly.GetCustomAttributes() select assem.CreateReference()).ToArray();
        }

        public static implicit operator AssemblyDescriptor(Assembly assembly)
        {
            return new AssemblyDescriptor(assembly);
        }

        //private readonly Cached<ITypeDescriptor>[] types;
        //private readonly Cached<IAssemblyDescriptor>[] references;


        //public AssemblyDescriptor(Assembly assembly)
        //    : base(DescriptorReference.CreateReference(assembly))
        //{

        //    types = (from type in assembly.GetTypes() select new Cached<ITypeDescriptor>(type)).ToArray();
        //    references = (from assem in assembly.GetReferencedAssemblies() select new Cached<IAssemblyDescriptor>(assem)).ToArray();
        //}



        //public ITypeDescriptor GetType(string name) { return GetType(name, false); }

        //public ITypeDescriptor GetType(string name, bool ignoreCase)
        //{
        //    Cached<ITypeDescriptor> firstOrDefault = (from t in types
        //                                              where t.Name.EndsWith(name, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture)
        //                                              select t).FirstOrDefault();
        //    return firstOrDefault != null ? firstOrDefault.Value : null;
        //}
        //public ITypeDescriptor[] GetTypes() { return Array.ConvertAll(types, e => e.Value); }

        //public object[] GetCustomAttributes(bool inherit) { throw new NotImplementedException(); }
        //public object[] GetCustomAttributes(ITypeDescriptor attributeType, bool inherit) { throw new NotImplementedException(); }

        //public IMethodDescriptor EntryPoint { get { throw new NotImplementedException(); } }
        //public IAssemblyDescriptor[] GetReferencedAssemblies() { return Array.ConvertAll(references, e => e.Value); }
    }

    [Serializable]
    public class TypeDescriptor : Descriptor
    {
        public override DescriptorType DescriptorType { get { return DescriptorType.Type; } }

        public TypeDescriptor(string url)
            : base(url)
        {
        }
    }


    public enum DescriptorType
    {
        Assembly,
        Type,
        Module,
        Method,
        Property,
        Field,
        Event,
        Constructor,
        Invalid
    }
}
