using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Descriptors.References;

namespace DotJEM.Reflection.Descriptors.Descriptors.Instance
{
    [Serializable]
    public class ObjectDescriptor : DynamicObject
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
 
        public ObjectDescriptor(object obj)
        {
            Type type = obj.GetType();

            foreach (PropertyInfo property in type.GetProperties())
            {
                SetProperty(property, property.GetValue(obj, null));
            }

            foreach (FieldInfo property in type.GetFields())
            {
                SetProperty(property, property.GetValue(obj));
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return values.TryGetValue(binder.Name, out result);
        }

        private void SetProperty(MemberInfo member, object value)
        {
            Type type = value.GetType();
            if (type.IsPrimitive || type.IsEnum || wellKnownTypes.Contains(type))
            {
                values[member.Name] = value;
            }
            else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
            {
                //TODO: Handle Arrays and Collections, wellknown .NET collections should just be added directly.
            }
            else
            {
                //TODO: This generates a stack overflow.
                //      one thing that could cause this is that we have cercular references.
                //values[member.Name] = new ObjectDescriptor(value);
            }
        }

        //public TypeDescriptor GetTypeDescriptor()
        //{
        //    return type.Resolve(this);
        //}

        private static readonly HashSet<Type> wellKnownTypes = new HashSet<Type> { typeof(string), typeof(Guid), typeof(DateTime), typeof(Point), typeof(PointF), typeof(Size), typeof(SizeF) };
    }

    [Serializable]
    public class AttributeDescriptor : ObjectDescriptor
    {
        public AttributeDescriptor(Attribute attribute) : base(attribute)
        {
        }
    }
}