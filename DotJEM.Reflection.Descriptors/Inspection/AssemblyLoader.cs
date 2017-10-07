using System;
using System.Diagnostics;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Descriptors;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;

namespace DotJEM.Reflection.Descriptors.Inspection
{
    internal class AssemblyLoader : MarshalByRefObject
    {
        public Descriptor Load(string uri) => Load(DescriptorUri.Parse(uri));

        private Descriptor Load(DescriptorUri uri)
        {
            if(!uri.IsValid)
                throw new ArgumentException(nameof(uri));

            //TODO: Check if it's already loaded.
            Assembly assembly = Assembly.LoadFrom(uri.AssemblyPath);
            Console.WriteLine(uri);

            switch (uri.Type)
            {
                case DescriptorType.Assembly:
                    return new AssemblyDescriptor(assembly);

                case DescriptorType.Type:
                    return LoadType(assembly, (TypeDescriptorUri) uri);

                case DescriptorType.Property:
                    return LoadProperty(assembly, (PropertyDescriptorUri) uri);

                default:
                    throw new ArgumentOutOfRangeException(nameof(uri));
            }
        }

        private TypeDescriptor LoadType(Assembly assembly, TypeDescriptorUri url)
        {
            return new TypeDescriptor(assembly.GetType(url.TypeName));
        }

        private Descriptor LoadProperty(Assembly assembly, PropertyDescriptorUri url)
        {
            TypeDescriptorUri typeUri = (TypeDescriptorUri)url.DeclaringType ;
            Type type = assembly.GetType(typeUri.TypeName);
            PropertyInfo property = type.GetProperty(url.Property);
            return new PropertyDescriptor(property);
        }


    }
}