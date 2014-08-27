using System;
using System.Diagnostics;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    public class AssemblyLoader : MarshalByRefObject
    {
        public AssemblyDescriptor LoadAssembly(string assemblyPath)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return new AssemblyDescriptor(assembly);
        }

        public TypeDescriptor LoadType(Assembly assembly, DescriptorUrl url)
        {
            //Assembly assembly = Load(url);

            return new TypeDescriptor(assembly.GetType(url.Type));
        }

        private Descriptor InternalLoad(DescriptorUrl url)
        {
            Assembly assembly = url.IsGac ? Assembly.Load(url.AssemblyName) : Assembly.LoadFrom(url.AssemblyLocation);
            Console.WriteLine(url);

            Debugger.Break();

            switch (url.DescriptorType)
            {
                case DescriptorType.Assembly:
                    return new AssemblyDescriptor(assembly);

                case DescriptorType.Type:
                    return LoadType(assembly, url);

                case DescriptorType.Property:
                    return null;//LoadProperty(url);
                default:
                    throw new ArgumentOutOfRangeException("url");
            }
        }

        public Descriptor Load(string url)
        {
            return InternalLoad(url);
        }

        //private PropertyDescriptor LoadProperty(ReferenceParts reference)
        //{
        //    Assembly assembly = Load(reference);
        //    Type type = assembly.GetType(reference.Type);
        //    PropertyInfo property = type.GetProperty(reference.Property);
        //    //System.String StringProperty
        //    return new PropertyDescriptor(property);
        //}

    }
}