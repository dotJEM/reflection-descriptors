using System;
using System.Diagnostics;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    public class AssemblyLoader : MarshalByRefObject
    {
        public Descriptor Load(DescriptorUrl url)
        {
            Assembly assembly = url.IsGac ? Assembly.Load(url.AssemblyName) : Assembly.LoadFrom(url.AssemblyLocation);
            Console.WriteLine(url);

            switch (url.DescriptorType)
            {
                case DescriptorType.Assembly:
                    return new AssemblyDescriptor(assembly);

                case DescriptorType.Type:
                    return LoadType(assembly, url);

                case DescriptorType.Property:
                    return LoadProperty(assembly, url);
                default:
                    throw new ArgumentOutOfRangeException("url");
            }
        }

        private TypeDescriptor LoadType(Assembly assembly, DescriptorUrl url)
        {
            return new TypeDescriptor(assembly.GetType(url.Type));
        }

        private Descriptor LoadProperty(Assembly assembly, DescriptorUrl url)
        {
            Type type = assembly.GetType(url.Type);
            PropertyInfo property = type.GetProperty(url.Property);
            return new PropertyDescriptor(property);
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