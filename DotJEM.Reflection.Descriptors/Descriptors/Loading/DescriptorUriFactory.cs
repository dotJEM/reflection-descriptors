using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{
    public static class DescriptorUriFactory
    {
        public static string CreateReference(this AssemblyName name)
        {
            return CreateReference(Assembly.Load(name));
            

            //string path = name.CodeBase.Replace("file:///", "");
            //return $"assembly://{new Uri(name.CodeBase).LocalPath}";
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

            return $"type://{type.FullName}@{CreateReference(type.Assembly)}";
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
            //return CreateReference(constructor.DeclaringType) + "&constructor=" + constructor;
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
        public override string AssemblyPath => Location;

        public AssemblyDescriptorUri(string value, string location)
            : base(value)
        {
            Location = location;
        }
    }

    public class TypeDescriptorUri : DescriptorUri
    {
        public string TypeName { get; }
        public DescriptorUri Assembly { get; }
        public override DescriptorType Type => DescriptorType.Type;
        public override string AssemblyPath => Assembly.AssemblyPath;

        public TypeDescriptorUri(string value, string type, DescriptorUri assembly)
            : base(value)
        {
            this.TypeName = type;
            this.Assembly = assembly;
        }
    }

    public class MethodDescriptorUri : DescriptorUri
    {
        public string Method { get; }
        public DescriptorUri DeclaringType { get; }
        public override DescriptorType Type => DescriptorType.Method;
        public override string AssemblyPath => DeclaringType.AssemblyPath;

        public MethodDescriptorUri(string value, string method, DescriptorUri declaringType)
            : base(value)
        {
            this.Method = method;
            this.DeclaringType = declaringType;
        }
    }

    public class PropertyDescriptorUri : DescriptorUri
    {
        public string Property { get; }
        public DescriptorUri DeclaringType { get; }

        public override DescriptorType Type => DescriptorType.Property;
        public override string AssemblyPath => DeclaringType.AssemblyPath;

        public PropertyDescriptorUri(string value, string property, DescriptorUri declaringType)
            : base(value)
        {
            this.Property = property;
            this.DeclaringType = declaringType;
        }
    }

    public class ConstructorDescriptorUri : DescriptorUri
    {
        public string Constructor { get; }
        public DescriptorUri DeclaringType { get; }
        public override DescriptorType Type => DescriptorType.Constructor;
        public override string AssemblyPath => DeclaringType.AssemblyPath;

        public ConstructorDescriptorUri(string value, string constructor, DescriptorUri declaringType)
            : base(value)
        {
            this.Constructor = constructor;
            this.DeclaringType = declaringType;
        }
    }

    public class EventDescriptorUri : DescriptorUri
    {
        public string Event { get; }
        public DescriptorUri DeclaringType { get; }
        public override DescriptorType Type => DescriptorType.Event;
        public override string AssemblyPath => DeclaringType.AssemblyPath;

        public EventDescriptorUri(string value, string @event, DescriptorUri declaringType) 
            : base(value)
        {
            this.Event = @event;
            this.DeclaringType = declaringType;
        }
    }

    public class FieldDescriptorUri : DescriptorUri
    {
        public string Field { get; }
        public DescriptorUri DeclaringType { get; }

        public override DescriptorType Type => DescriptorType.Field;
        public override string AssemblyPath => DeclaringType.AssemblyPath;

        public FieldDescriptorUri(string value, string field, DescriptorUri declaringType)
            : base(value)
        {
            this.Field = field;
            this.DeclaringType = declaringType;
        }
    }

    public class InvalidDescriptorUri : DescriptorUri
    {
        public override DescriptorType Type => DescriptorType.Invalid;
        public override string AssemblyPath => throw new InvalidOperationException("Invalid descriptor encountered.");

        public InvalidDescriptorUri(string value) : base(value)
        {
        }
    }

    public abstract class DescriptorUri
    {
        private static readonly Regex pattern = new Regex("(?'type'\\w+)\\:\\/\\/(?'val'[^@]+)(@(?'at'.*))?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public abstract DescriptorType Type { get; }
        public string Value { get; }
        public abstract string AssemblyPath { get; }

        public bool IsValid => Type != DescriptorType.Invalid;

        protected DescriptorUri(string value)
        {
            Value = value;
        }

        public static implicit operator DescriptorUri(string uri) => Parse(uri);

        public static DescriptorUri Parse(string uri)
        {
            Match match = pattern.Match(uri);
            if (!match.Success)
                return new InvalidDescriptorUri(uri);

            bool TryParsePart(out DescriptorUri outpart)
            {
                outpart = null;
                string typePart = match.Groups["at"].Value;
                if (string.IsNullOrEmpty(typePart))
                    return false;

                outpart = Parse(typePart);
                return outpart.IsValid;
            }

            DescriptorUri part = null;
            switch (match.Groups["type"].Value.ToLowerInvariant())
            {
                //Assembly
                case "assembly":
                    return new AssemblyDescriptorUri(uri, match.Groups["val"].Value);

                //Type
                case "type":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new TypeDescriptorUri(uri, match.Groups["val"].Value, part);

                //Method
                case "method":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new MethodDescriptorUri(uri, match.Groups["val"].Value, part);

                //Property
                case "property":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new PropertyDescriptorUri(uri, match.Groups["val"].Value, part);

                //Constructor
                case "constructor":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new ConstructorDescriptorUri(uri, match.Groups["val"].Value, part);

                //Field
                case "field":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new FieldDescriptorUri(uri, match.Groups["val"].Value, part);

                //Event
                case "event":
                    if (!TryParsePart(out part))
                        return new InvalidDescriptorUri(uri);

                    return new EventDescriptorUri(uri, match.Groups["val"].Value, part);
            }
            return new InvalidDescriptorUri(uri);
        }



        //Module,
        //Attribute,
        //Object,
        //Invalid    
    }

}