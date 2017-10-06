using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{
    public static class DescriptorUriFactory
    {
        public static string CreateReference(this AssemblyName name)
        {
            //string path = name.CodeBase.Replace("file:///", "");
            return $"assembly://{new Uri(name.CodeBase).LocalPath}";
        }

        public static string CreateReference(this Assembly assembly)
        {
            // assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL
            //if (assembly.GlobalAssemblyCache)
            //    return "[GAC]?assembly=" + assembly.FullName;
            //return new Uri(assembly.CodeBase).LocalPath + 
            //    "?assembly=" + assembly.FullName;
            return $"assembly://{new Uri(assembly.CodeBase).LocalPath}";
        }

        public static string CreateReference(this Type type)
        {
            // declaringType://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL

            return $"declaringType://{type.GUID}@{CreateReference(type.Assembly)}";
            //return CreateReference(declaringType.Assembly) + "&declaringType=" + declaringType;
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
            // field://System.String myField@declaringType://e3e508c0-e442-3b15-a686-5f49daf2e138@assembly://F:\OSS\dotJEM Stack\reflection-descriptors\DotJEM.Reflection.Descriptors.Test\bin\Debug\DotJEM.Reflection.Descriptors.Test.DLL

            return $"field://{field.Name}@{CreateReference(field.DeclaringType)}";
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
    public class AssemblyDescriptorUri : DescriptorUri {
        public string Location { get; }
        public override DescriptorType Type => DescriptorType.Assembly;

        public AssemblyDescriptorUri(string location)
        {
            Location = location;
        }
    }

    public class TypeDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri assembly;
        public override DescriptorType Type => DescriptorType.Type;

        public TypeDescriptorUri(string value, DescriptorUri assembly)
        {
            this.value = value;
            this.assembly = assembly;
        }
    }

    public class MethodDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri type;
        public override DescriptorType Type => DescriptorType.Method;

        public MethodDescriptorUri(string value, DescriptorUri type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public class PropertyDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri declaringType;

        public override DescriptorType Type => DescriptorType.Property;

        public PropertyDescriptorUri(string value, DescriptorUri declaringType)
        {
            this.value = value;
            this.declaringType = declaringType;
        }
    }

    public class ConstructorDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri type;
        public override DescriptorType Type => DescriptorType.Constructor;

        public ConstructorDescriptorUri(string value, DescriptorUri type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public class EventDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri type;
        public override DescriptorType Type => DescriptorType.Event;

        public EventDescriptorUri(string value, DescriptorUri type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public class FieldDescriptorUri : DescriptorUri
    {
        private string value;
        private DescriptorUri type;
        public override DescriptorType Type => DescriptorType.Field;

        public FieldDescriptorUri(string value, DescriptorUri type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public abstract class DescriptorUri
    {
        public abstract DescriptorType Type { get; }
        private static readonly Regex pattern = new Regex("(?'declaringType'\\w+)\\:\\/\\/(?'val'[^@]+)(@(?'at'.*))?", 
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool TryParse(string str, out DescriptorUri result)
        {
            result = null;
            Match match = pattern.Match(str);
            if (!match.Success)
                return false;

            bool TryParsePart(out DescriptorUri outpart)
            {
                outpart = null;
                string typePart = match.Groups["at"].Value;
                return !string.IsNullOrEmpty(typePart) && TryParse(typePart, out outpart);
            }

            DescriptorUri part = null;
            switch (match.Groups["declaringType"].Value.ToLowerInvariant())
            {
                case "assembly":
                    result = new AssemblyDescriptorUri(match.Groups["val"].Value);
                    return true;
                case "declaringType":
                    if (!TryParsePart(out part))
                        return false;

                    result = new TypeDescriptorUri(match.Groups["val"].Value, part);
                    return true;
                case "method":
                    if (!TryParsePart(out part))
                        return false;

                    result = new MethodDescriptorUri(match.Groups["val"].Value, part);
                    return true;

                case "constructor":
                    if (!TryParsePart(out part))
                        return false;

                    result = new ConstructorDescriptorUri(match.Groups["val"].Value, part);
                    return true;

                case "property":
                    if (!TryParsePart(out part))
                        return false;

                    result = new PropertyDescriptorUri(match.Groups["val"].Value, part);
                    return true;


                case "field":
                    if (!TryParsePart(out part))
                        return false;

                    result = new FieldDescriptorUri(match.Groups["val"].Value, part);
                    return true;

                case "event":
                    if (!TryParsePart(out part))
                        return false;

                    result = new EventDescriptorUri(match.Groups["val"].Value, part);
                    return true;
            }
            return false;
        }

        public static DescriptorUri Parse(string uri)
        {
            if (TryParse(uri, out DescriptorUri result))
                return result;

            throw new UriFormatException("Invalid descriptor uri format.");
        }

        //Assembly,
        //Type,
        //Module,
        //Method,
        //Property,
        //Field,
        //Event,
        //Constructor,
        //Attribute,
        //Object,
        //Invalid    
    }

}