using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace DotJEM.Reflection.Descriptors.Descriptors
{
    [Serializable]
    public class DescriptorUrl : ISerializable 
    {
        private readonly IDictionary<string, DescriptorUriElement> parts;
       
        public string Url { get; set; }
        public DescriptorType DescriptorType { get; set; }

        public bool IsGac { get { return AssemblyLocation == "[GAC]"; } }
        public bool IsEmpty { get { return string.IsNullOrWhiteSpace(Url); } }
        public bool IsValid { get; private set; }

        public string AssemblyLocation { get { return IsValid ? parts["AssemblyLocation"].Value : ""; } }

        public DescriptorUrl(string url)
        {
            Url = url;
            IsValid = DescriptorUriParser.TryParse(url, out parts);
            DescriptorType = IsValid ? DescriptorUriParser.GetDescriptorType(parts) : DescriptorType.Invalid;
        }

        #region Object Overrides
        protected bool Equals(DescriptorUrl other)
        {
            return string.Equals(uri, other.uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DescriptorUrl)obj);
        }

        public override int GetHashCode()
        {
            return (uri != null ? uri.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return uri;
        } 
        #endregion

        #region Implicit Conversions
        public static implicit operator string(DescriptorUrl url)
        {
            return url.ToString();
        }

        public static implicit operator DescriptorUrl(string uri)
        {
            return new DescriptorUrl(uri);
        } 
        #endregion

        #region Serialization Members
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("url", uri);
        }

        protected DescriptorUrl(SerializationInfo info, StreamingContext context)
            : this(info.GetString("url"))
        {
        } 
        #endregion
    }

    internal class DescriptorUriElement
    {
        public string Type { get; private set; }
        public string Value { get; private set; }

        public DescriptorUriElement(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    internal static class DescriptorUriParser
    {
        public static bool TryParse(string uri, out IDictionary<string, DescriptorUriElement> map)
        {
            string[] groups = uri.Split('&', '?');

            map = new Dictionary<string, DescriptorUriElement>();
            if (groups.Length < 2)
                return false;

            map.Add("AssemblyLocation", new DescriptorUriElement("AssemblyLocation", groups[0]));
            map.Add("AssemblyName", new DescriptorUriElement("AssemblyName", groups[1]));
            for (int i = 2; i < groups.Length; i++)
            {
                //TODO: validation of groups.
                int split = groups[i].IndexOf('=');
                string key = groups[i].Substring(0, split);
                map.Add(key, new DescriptorUriElement(key, groups[i].Substring(split+1)));
            }

            return true;
        }

        //...&type=System.Collections.Generic.List`1[T]
        //...&type=System.Collections.Generic.List`1[System.Int32]
        //...&method=Int32 Bot1[T](T)
        //...&method=Int32 Bot2[T,X](T, X, Int32)
        //...&method=Int32 Bot3[T,X](T, X, Int32)

        //public MemberTypes GetMemberType()
        //{
        //    if (map.ContainsKey("type"))
        //    {
        //        if (map.ContainsKey("field")) return MemberTypes.Field;
        //        if (map.ContainsKey("property")) return MemberTypes.Property;
        //        if (map.ContainsKey("event")) return MemberTypes.Event;
        //        if (map.ContainsKey("method")) return MemberTypes.Method;
        //        if (map.ContainsKey("ctor")) return MemberTypes.Constructor;
        //        return MemberTypes.TypeInfo;
        //    }
        //    return MemberTypes.Custom;
        //}

        public static DescriptorType GetDescriptorType(IDictionary<string, DescriptorUriElement> map)
        {
            if (map.ContainsKey("type"))
            {
                if (map.ContainsKey("field")) return DescriptorType.Field;
                if (map.ContainsKey("property")) return DescriptorType.Property;
                if (map.ContainsKey("event")) return DescriptorType.Event;
                if (map.ContainsKey("method")) return DescriptorType.Method;
                if (map.ContainsKey("ctor")) return DescriptorType.Constructor;
                return DescriptorType.Type;
            }
            if (map.ContainsKey("module"))
                return DescriptorType.Module;
            
            return DescriptorType.Assembly;
        }
    }










}