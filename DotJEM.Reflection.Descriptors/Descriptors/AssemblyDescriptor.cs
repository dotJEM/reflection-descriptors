using System;
using System.Linq;
using System.Reflection;
using DotJEM.Reflection.Descriptors.Cache;
using DotJEM.Reflection.Descriptors.Descriptors.Instance;
using DotJEM.Reflection.Descriptors.Descriptors.Loading;
using DotJEM.Reflection.Descriptors.Descriptors.References;

namespace DotJEM.Reflection.Descriptors.Descriptors
{
    [Serializable]
    public abstract class Descriptor
    {
        public DescriptorUrl Url { get; }
        public abstract DescriptorType DescriptorType { get; }

        internal LoadInfo LoadInfo { get; set; }
        internal DescriptorCache Cache { get; set; }

        protected Descriptor(string url)
        {
            Url = url;
        }
    }

    [Serializable]
    public class AssemblyDescriptor : Descriptor
    {
        public bool IsDynamic { get; }
        public bool IsFullyTrusted { get; }
        public bool GlobalAssemblyCache { get; }

        public string FullName { get; }
        public string Location { get; }
        public string CodeBase { get; }

        public override DescriptorType DescriptorType => DescriptorType.Assembly;

        private readonly ArrayRef<TypeDescriptor> types;
        private readonly ArrayRef<AssemblyDescriptor> assemblies;

        public TypeDescriptor[] Types => types.Resolve(this);
        public AssemblyDescriptor[] ReferencedAssemblies => assemblies.Resolve(this);

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
    public abstract class MemberDescriptor : Descriptor
    {
        public string Name { get; }
        public int MetadataToken { get; }
        public MemberTypes MemberType { get; }

        private readonly AttributeDescriptor[] customAttributes;

        protected MemberDescriptor(MemberInfo member, string url)
            : base(url)
        {
            MetadataToken = member.MetadataToken;
            Name = member.Name;
            MemberType = member.MemberType;

            //TODO: Use ArrayRef<T> or a special AttributesRef<T>
            customAttributes = (from obj in member.GetCustomAttributes(false)
                                let attribute = obj as Attribute
                                where attribute != null
                                select new AttributeDescriptor(attribute)).ToArray();
        }

        public virtual AttributeDescriptor[] GetCustomAttributes(bool inherit)
        {
            //TODO: Overload with call to "base" for types etc??
            return customAttributes;
        }
    }
    //[Serializable]
    //public class MemberDescriptor : Descriptor, IMemberDescriptor
    //{
    //    private readonly Cached<ITypeDescriptor> declaringType;
    //    private readonly Cached<ITypeDescriptor> reflectedType;
    //    private readonly Cached<IModuleDescriptor> module;

    //    private readonly AttributeDescriptor[] customAttributes;

    //    public string Name { get; private set; }
    //    public int MetadataToken { get; private set; }
    //    public MemberTypes MemberType { get; private set; }

    //    public ITypeDescriptor DeclaringType { get { return declaringType.Value; } }
    //    public ITypeDescriptor ReflectedType { get { return reflectedType.Value; } }

    //    public IModuleDescriptor Module { get { return module.Value; } }

    //    public MemberDescriptor(MemberInfo member)
    //        : base(DescriptorReference.Create(member))
    //    {
    //        MetadataToken = member.MetadataToken;
    //        Name = member.Name;
    //        MemberType = member.MemberType;

    //        declaringType = new Cached<ITypeDescriptor>(member.DeclaringType);
    //        reflectedType = new Cached<ITypeDescriptor>(member.ReflectedType);

    //        module = new Cached<IModuleDescriptor>(member.Module);

    //        customAttributes = (from obj in member.GetCustomAttributes(false)
    //                            let attribute = obj as Attribute
    //                            where attribute != null
    //                            select new AttributeDescriptor(attribute)).ToArray();
    //    }

    //    public virtual IAttributeDescriptor[] GetCustomAttributes(bool inherit)
    //    {
    //        //TODO: Overload with call to "base" for types etc??
    //        return customAttributes;
    //    }

    //    public IAttributeDescriptor[] GetCustomAttributes(ITypeDescriptor attributeType, bool inherit)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool IsDefined(ITypeDescriptor attributeType, bool inherit)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //[Serializable]
    //public class MemberDescriptor : Descriptor, IMemberDescriptor
    //{
    //    private readonly Cached<ITypeDescriptor> declaringType;
    //    private readonly Cached<ITypeDescriptor> reflectedType;
    //    private readonly Cached<IModuleDescriptor> module;

    //    private readonly AttributeDescriptor[] customAttributes;

    //    public string Name { get; private set; }
    //    public int MetadataToken { get; private set; }
    //    public MemberTypes MemberType { get; private set; }

    //    public ITypeDescriptor DeclaringType { get { return declaringType.Value; } }
    //    public ITypeDescriptor ReflectedType { get { return reflectedType.Value; } }

    //    public IModuleDescriptor Module { get { return module.Value; } }

    //    public MemberDescriptor(MemberInfo member)
    //        : base(DescriptorReference.Create(member))
    //    {
    //        MetadataToken = member.MetadataToken;
    //        Name = member.Name;
    //        MemberType = member.MemberType;

    //        declaringType = new Cached<ITypeDescriptor>(member.DeclaringType);
    //        reflectedType = new Cached<ITypeDescriptor>(member.ReflectedType);

    //        module = new Cached<IModuleDescriptor>(member.Module);

    //        customAttributes = (from obj in member.GetCustomAttributes(false)
    //                            let attribute = obj as Attribute
    //                            where attribute != null
    //                            select new AttributeDescriptor(attribute)).ToArray();
    //    }

    //    public virtual IAttributeDescriptor[] GetCustomAttributes(bool inherit)
    //    {
    //        //TODO: Overload with call to "base" for types etc??
    //        return customAttributes;
    //    }

    //    public IAttributeDescriptor[] GetCustomAttributes(ITypeDescriptor attributeType, bool inherit)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool IsDefined(ITypeDescriptor attributeType, bool inherit)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}



    [Serializable]
    public class TypeDescriptor : MemberDescriptor
    {
        private readonly ObjectRef<AssemblyDescriptor> assembly;
        private readonly ObjectRef<TypeDescriptor> baseType;
        private readonly ObjectRef<TypeDescriptor> underlyingSystemType;

        private readonly ArrayRef<TypeDescriptor> interfaces; 
        private readonly ArrayRef<PropertyDescriptor> properties;

        #region Simple Properties
        public string FullName { get; }
        public string Namespace { get; }
        public string AssemblyQualifiedName { get; }

        public int GenericParameterPosition { get; }

        public Guid Guid { get; }

        public TypeAttributes Attributes { get; }
        public GenericParameterAttributes GenericParameterAttributes { get; }
        #endregion

        #region Boolean Flags
        public bool IsNested { get; }
        public bool IsVisible { get; }
        public bool IsNotPublic { get; }
        public bool IsPublic { get; }
        public bool IsNestedPublic { get; }
        public bool IsNestedPrivate { get; }
        public bool IsNestedFamily { get; }
        public bool IsNestedAssembly { get; }
        public bool IsNestedFamANDAssem { get; }
        public bool IsNestedFamORAssem { get; }
        public bool IsAutoLayout { get; }
        public bool IsLayoutSequential { get; }
        public bool IsExplicitLayout { get; }
        public bool IsClass { get; }
        public bool IsInterface { get; }
        public bool IsValueType { get; }
        public bool IsAbstract { get; }
        public bool IsSealed { get; }
        public bool IsEnum { get; }
        public bool IsSpecialName { get; }
        public bool IsImport { get; }
        public bool IsSerializable { get; }
        public bool IsAnsiClass { get; }
        public bool IsUnicodeClass { get; }
        public bool IsAutoClass { get; }
        public bool IsArray { get; }
        public bool IsGenericType { get; }
        public bool IsGenericTypeDefinition { get; }
        public bool IsGenericParameter { get; }
        public bool ContainsGenericParameters { get; }
        public bool IsByRef { get; }
        public bool IsPointer { get; }
        public bool IsPrimitive { get; }
        public bool IsCOMObject { get; }
        public bool HasElementType { get; }
        public bool IsContextful { get; }
        public bool IsMarshalByRef { get; }
        public bool IsSecurityCritical { get; }
        public bool IsSecuritySafeCritical { get; }
        public bool IsSecurityTransparent { get; }
        #endregion

        public AssemblyDescriptor Assembly => assembly.Resolve(this);
        public TypeDescriptor BaseType => baseType.Resolve(this);
        public TypeDescriptor UnderlyingSystemType => underlyingSystemType.Resolve(this);

        public override DescriptorType DescriptorType => DescriptorType.Type;

        public TypeDescriptor[] Interfaces => interfaces.Resolve(this);
        public PropertyDescriptor[] Properties => properties.Resolve(this);

        public TypeDescriptor(Type type)
            : base(type, type.CreateReference())
        {
            #region Simple Properties

            FullName = type.FullName;
            Namespace = type.Namespace;
            AssemblyQualifiedName = type.AssemblyQualifiedName;

            Guid = type.GUID;
            Attributes = type.Attributes;

            //TODO: Must not be called if IsGenericParameter is not true.
            GenericParameterPosition = type.IsGenericParameter ? type.GenericParameterPosition : -1;
            GenericParameterAttributes = type.IsGenericParameter ? type.GenericParameterAttributes : GenericParameterAttributes.None;
            #endregion

            #region Boolean Flags
            IsNested = type.IsNested;
            IsVisible = type.IsVisible;
            IsNotPublic = type.IsNotPublic;
            IsPublic = type.IsPublic;
            IsNestedPublic = type.IsNestedPublic;
            IsNestedPrivate = type.IsNestedPrivate;
            IsNestedFamily = type.IsNestedFamily;
            IsNestedAssembly = type.IsNestedAssembly;
            IsNestedFamANDAssem = type.IsNestedFamANDAssem;
            IsNestedFamORAssem = type.IsNestedFamORAssem;
            IsAutoLayout = type.IsAutoLayout;
            IsLayoutSequential = type.IsLayoutSequential;
            IsExplicitLayout = type.IsExplicitLayout;
            IsClass = type.IsClass;
            IsInterface = type.IsInterface;
            IsValueType = type.IsValueType;
            IsAbstract = type.IsAbstract;
            IsSealed = type.IsSealed;
            IsEnum = type.IsEnum;
            IsSpecialName = type.IsSpecialName;
            IsImport = type.IsImport;
            IsSerializable = type.IsSerializable;
            IsAnsiClass = type.IsAnsiClass;
            IsUnicodeClass = type.IsUnicodeClass;
            IsAutoClass = type.IsAutoClass;
            IsArray = type.IsArray;
            IsGenericType = type.IsGenericType;
            IsGenericTypeDefinition = type.IsGenericTypeDefinition;
            IsGenericParameter = type.IsGenericParameter;
            ContainsGenericParameters = type.ContainsGenericParameters;
            IsByRef = type.IsByRef;
            IsPointer = type.IsPointer;
            IsPrimitive = type.IsPrimitive;
            IsCOMObject = type.IsCOMObject;
            HasElementType = type.HasElementType;
            IsContextful = type.IsContextful;
            IsMarshalByRef = type.IsMarshalByRef;
            IsSecurityCritical = type.IsSecurityCritical;
            IsSecuritySafeCritical = type.IsSecuritySafeCritical;
            IsSecurityTransparent = type.IsSecurityTransparent;
            #endregion

            baseType = type.BaseType.CreateReference();
            underlyingSystemType = type.UnderlyingSystemType.CreateReference();
            assembly = type.Assembly.CreateReference();

            ////members = (from member in type.GetMembers() select Cached<IMemberDescriptor>.Create(member));

            //constructors = (from ctorInfo in type.GetConstructors() select new Cached<IConstructorDescriptor>(ctorInfo)).ToArray();
            properties = (from propertyInfo in type.GetProperties() select propertyInfo.CreateReference()).ToArray();
            interfaces = (from typeInfo in type.GetInterfaces() select typeInfo.CreateReference()).ToArray();
            //methods = (from methodInfo in type.GetMethods() select new Cached<IMethodDescriptor>(methodInfo)).ToArray();

        }

        public static implicit operator TypeDescriptor(Type type)
        {
            return new TypeDescriptor(type);
        }

        public bool IsSubclassOf(TypeDescriptor type)
        {
            return false;
        }


    }

    [Serializable]
    public class PropertyDescriptor : MemberDescriptor
    {
        public override DescriptorType DescriptorType => DescriptorType.Property;

        private readonly ObjectRef<TypeDescriptor> propertyType;

        public TypeDescriptor PropertyType => propertyType.Resolve(this);


        public PropertyDescriptor(PropertyInfo property)
            : base(property, property.CreateReference())
        {
            propertyType = property.PropertyType.CreateReference();
        }
    }

    [Serializable]
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
        Attribute,
        Object,
        Invalid
    }
}
