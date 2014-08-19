using System;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Descriptors;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    public class AssemblyLoader : MarshalByRefObject
    {
        public AssemblyDescriptor LoadAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return new AssemblyDescriptor(assembly);
        }

        //public AssemblyDescriptor LoadAssembly(DescriptorUri uri)
        //{
        //    return new AssemblyDescriptor(Load(uri));
        //}

        //public TypeDescriptor LoadType(DescriptorUri reference)
        //{
        //    Assembly assembly = Load(reference);

        //    return new TypeDescriptor(assembly.GetType(reference.Type));
        //}

        public Descriptor Load(DescriptorUrl url)
        {
            switch (url.DescriptorType)
            {
                case DescriptorType.Assembly:
                    return LoadAssembly(url);
                case DescriptorType.Type:
                    return LoadType(url);
                case DescriptorType.Property:
                    return LoadProperty(url);
                default:
                    throw new ArgumentOutOfRangeException("descriptorType");
            }
        }

        //private PropertyDescriptor LoadProperty(ReferenceParts reference)
        //{
        //    Assembly assembly = Load(reference);
        //    Type type = assembly.GetType(reference.Type);
        //    PropertyInfo property = type.GetProperty(reference.Property);
        //    //System.String StringProperty
        //    return new PropertyDescriptor(property);
        //}

        //private Assembly Load(ReferenceParts reference)
        //{
        //    if (reference.IsGac)
        //        return Assembly.Load(reference.AssemblyName);
        //    return Assembly.LoadFrom(reference.AssemblyLocation);
        //}
    }
}