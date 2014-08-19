using System;
using System.Reflection;

namespace DotJEM.Reflection.Descriptors.Descriptors
{
    internal static class DescriptorUriFactory
    {
        public static string CreateReference(this AssemblyName name)
        {
            return CreateReference(Assembly.Load(name));
        }

        public static string CreateReference(this Assembly assembly)
        {
            if (assembly.GlobalAssemblyCache)
                return "[GAC]?assembly=" + assembly.FullName;
            return new Uri(assembly.CodeBase).LocalPath + "?assembly=" + assembly.FullName;
        }

        public static string CreateReference(this Type type)
        {
            return CreateReference(type.Assembly) + "&type=" + type;
        }

        public static string CreateReference(this MethodInfo method)
        {
            return CreateReference(method.DeclaringType) + "&method=" + method;
        }

        public static string CreateReference(this ConstructorInfo constructor)
        {
            return CreateReference(constructor.DeclaringType) + "&constructor=" + constructor;
        }

        public static string CreateReference(this PropertyInfo property)
        {
            return CreateReference(property.DeclaringType) + "&property=" + property;
        }

        public static string CreateReference(this FieldInfo field)
        {
            return CreateReference(field.DeclaringType) + "&field=" + field;
        }

        public static string CreateReference(this EventInfo @event)
        {
            return CreateReference(@event.DeclaringType) + "&event=" + @event;
        }

        //public static string CreateReference(this MemberInfo member)
        //{
        //    return CreateReference((dynamic)member);
        //}

        public static string CreateReference(this Module module)
        {
            throw new NotImplementedException();
        }
    }
}