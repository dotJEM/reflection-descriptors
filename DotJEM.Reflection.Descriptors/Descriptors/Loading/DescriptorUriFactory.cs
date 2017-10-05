using System;
using System.Reflection;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{
    internal static class DescriptorUriFactory
    {
        public static string CreateReference(this AssemblyName name)
        {
            //string path = name.CodeBase.Replace("file:///", "");
            return $"assembly://{new Uri(name.CodeBase).LocalPath}";
        }

        public static string CreateReference(this Assembly assembly)
        {
            //if (assembly.GlobalAssemblyCache)
            //    return "[GAC]?assembly=" + assembly.FullName;
            //return new Uri(assembly.CodeBase).LocalPath + 
            //    "?assembly=" + assembly.FullName;
            return $"assembly://{new Uri(assembly.CodeBase).LocalPath}";
        }

        public static string CreateReference(this Type type)
        {
            return $"type://{type.GUID}@{CreateReference(type.Assembly)}";
            //return CreateReference(type.Assembly) + "&type=" + type;
        }

        public static string CreateReference(this MethodInfo method)
        {
            return $"method://{method}@{CreateReference(method.DeclaringType)}";
            //return CreateReference(method.DeclaringType) + "&method=" + method;
        }

        public static string CreateReference(this ConstructorInfo constructor)
        {
            return $"constructor://{constructor}@{CreateReference(constructor.DeclaringType)}";
            //return CreateReference(constructor.DeclaringType) + "&ctor=" + constructor;
        }

        public static string CreateReference(this PropertyInfo property)
        {
            return $"property://{property}@{CreateReference(property.DeclaringType)}";
            //return CreateReference(property.DeclaringType) + "&property=" + property;
        }

        public static string CreateReference(this FieldInfo field)
        {
            return $"field://{field}@{CreateReference(field.DeclaringType)}";
            //return CreateReference(field.DeclaringType) + "&field=" + field;
        }

        public static string CreateReference(this EventInfo @event)
        {
            return $"event://{@event}@{CreateReference(@event.DeclaringType)}";
           // return CreateReference(@event.DeclaringType) + "&event=" + @event;
        }

        //public static string CreateReference(this MemberInfo member)
        //{
        //    return CreateReference((dynamic)member);
        //}

        //public static string CreateReference(this Module module)
        //{
        //    throw new NotImplementedException();
        //}
    }
}