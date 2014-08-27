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
        public DescriptorUrl Url { get; private set; }
        public abstract DescriptorType DescriptorType { get; }

        internal DescriptorLoadInfo LoadInfo { get; set; }
        internal DescriptorCache Cache { get; set; }

        internal protected Descriptor(string url)
        {
            Url = url;
        }
    }

    [Serializable]
    public class AssemblyDescriptor : Descriptor
    {
        public bool IsDynamic { get; private set; }
        public bool IsFullyTrusted { get; private set; }
        public bool GlobalAssemblyCache { get; private set; }

        public string FullName { get; private set; }
        public string Location { get; private set; }
        public string CodeBase { get; private set; }

        public override DescriptorType DescriptorType { get { return DescriptorType.Assembly; } }

        private readonly ArrayRef<TypeDescriptor> types;
        private readonly ArrayRef<AssemblyDescriptor> assemblies;

        public TypeDescriptor[] Types { get { return types.Resolve(this); } }
        public AssemblyDescriptor[] ReferencedAssemblies { get { return assemblies.Resolve(this); } }

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
        public string Name { get; private set; }
        public int MetadataToken { get; private set; }
        public MemberTypes MemberType { get; private set; }

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

        private readonly ArrayRef<PropertyDescriptor> properties;

        #region Simple Properties
        public string FullName { get; private set; }
        public string Namespace { get; private set; }
        public string AssemblyQualifiedName { get; private set; }

        public int GenericParameterPosition { get; private set; }

        public Guid Guid { get; private set; }

        public TypeAttributes Attributes { get; private set; }
        public GenericParameterAttributes GenericParameterAttributes { get; private set; }
        #endregion

        #region Boolean Flags
        public bool IsNested { get; private set; }
        public bool IsVisible { get; private set; }
        public bool IsNotPublic { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsNestedPublic { get; private set; }
        public bool IsNestedPrivate { get; private set; }
        public bool IsNestedFamily { get; private set; }
        public bool IsNestedAssembly { get; private set; }
        public bool IsNestedFamANDAssem { get; private set; }
        public bool IsNestedFamORAssem { get; private set; }
        public bool IsAutoLayout { get; private set; }
        public bool IsLayoutSequential { get; private set; }
        public bool IsExplicitLayout { get; private set; }
        public bool IsClass { get; private set; }
        public bool IsInterface { get; private set; }
        public bool IsValueType { get; private set; }
        public bool IsAbstract { get; private set; }
        public bool IsSealed { get; private set; }
        public bool IsEnum { get; private set; }
        public bool IsSpecialName { get; private set; }
        public bool IsImport { get; private set; }
        public bool IsSerializable { get; private set; }
        public bool IsAnsiClass { get; private set; }
        public bool IsUnicodeClass { get; private set; }
        public bool IsAutoClass { get; private set; }
        public bool IsArray { get; private set; }
        public bool IsGenericType { get; private set; }
        public bool IsGenericTypeDefinition { get; private set; }
        public bool IsGenericParameter { get; private set; }
        public bool ContainsGenericParameters { get; private set; }
        public bool IsByRef { get; private set; }
        public bool IsPointer { get; private set; }
        public bool IsPrimitive { get; private set; }
        public bool IsCOMObject { get; private set; }
        public bool HasElementType { get; private set; }
        public bool IsContextful { get; private set; }
        public bool IsMarshalByRef { get; private set; }
        public bool IsSecurityCritical { get; private set; }
        public bool IsSecuritySafeCritical { get; private set; }
        public bool IsSecurityTransparent { get; private set; }
        #endregion

        public AssemblyDescriptor Assembly { get { return assembly.Resolve(this); } }
        public TypeDescriptor BaseType { get { return baseType.Resolve(this); } }
        public TypeDescriptor UnderlyingSystemType { get { return underlyingSystemType.Resolve(this); } }

        public override DescriptorType DescriptorType { get { return DescriptorType.Type; } }

        public PropertyDescriptor[] Properties { get { return properties.Resolve(this); } }

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

    public class PropertyDescriptor : MemberDescriptor
    {
        public override DescriptorType DescriptorType { get { return DescriptorType.Property; } }

        private readonly ObjectRef<TypeDescriptor> propertyType;

        public TypeDescriptor PropertyType { get { return propertyType.Resolve(this); } }


        public PropertyDescriptor(PropertyInfo property)
            : base(property, property.CreateReference())
        {
            propertyType = property.PropertyType.CreateReference();
        }
    }

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
