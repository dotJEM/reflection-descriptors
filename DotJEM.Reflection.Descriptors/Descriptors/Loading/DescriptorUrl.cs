using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace DotJEM.Reflection.Descriptors.Descriptors.Loading
{

    /// Descriptor URLS:
    /// 
    /// assembly://C:/...
    /// assembly://./
    /// type://

    //[Serializable]
    //public class DescriptorUrl : ISerializable
    //{
    //    private readonly IDictionary<string, DescriptorUriElement> parts;
       
    //    public string Url { get; set; }
    //    public DescriptorType DescriptorType { get; set; }

    //    public bool IsGac => AssemblyLocation == "[GAC]";
    //    public bool IsEmpty => string.IsNullOrWhiteSpace(Url);
    //    public bool IsValid { get; }

    //    public string AssemblyLocation => IsValid ? parts["AssemblyLocation"] : "";
    //    public string AssemblyName => IsValid ? parts["AssemblyName"] : "";
    //    public string Type => IsValid ? parts["Type"] : "";
    //    public string Property => IsValid ? parts["Property"] : "";

    //    public DescriptorUrl(string url)
    //    {
    //        Url = url;
    //        IsValid = DescriptorUriParser.TryParse(url, out parts);
    //        DescriptorType = IsValid ? DescriptorUriParser
    //            .GetDescriptorType(parts) : DescriptorType.Invalid;
    //    }

    //    #region Object Overrides
    //    protected bool Equals(DescriptorUrl other)
    //    {
    //        return string.Equals(Url, other.Url);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (ReferenceEquals(null, obj)) return false;
    //        if (ReferenceEquals(this, obj)) return true;
    //        if (obj.GetType() != GetType()) return false;
    //        return Equals((DescriptorUrl)obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return (Url != null ? Url.GetHashCode() : 0);
    //    }

    //    public override string ToString()
    //    {
    //        return Url;
    //    } 
    //    #endregion

    //    #region Implicit Conversions
    //    public static implicit operator string(DescriptorUrl url)
    //    {
    //        return url.ToString();
    //    }

    //    public static implicit operator DescriptorUrl(string uri)
    //    {
    //        return new DescriptorUrl(uri);
    //    } 
    //    #endregion

    //    #region Serialization Members
    //    public void GetObjectData(SerializationInfo info, StreamingContext context)
    //    {
    //        info.AddValue("url", Url);
    //    }

    //    protected DescriptorUrl(SerializationInfo info, StreamingContext context)
    //        : this(info.GetString("url"))
    //    {
    //    } 
    //    #endregion
    //}

    //internal class DescriptorUriElement
    //{
    //    public string Type { get; private set; }
    //    public string Value { get; private set; }

    //    public DescriptorUriElement(string type, string value)
    //    {
    //        Type = type;
    //        Value = value;
    //    }

    //    public static implicit operator string(DescriptorUriElement parts)
    //    {
    //        return parts.ToString();
    //    }

    //    public override string ToString()
    //    {
    //        return Value;
    //    }
    //}

    //internal class PropertyDescriptorUriElement : DescriptorUriElement
    //{
    //    public string Name { get; set; }
    //    public string ReturnType { get; set; }

    //    public PropertyDescriptorUriElement(string key, string value)
    //        : base(key, value)
    //    {
    //        string[] segments = value.Split(' ');
    //        Name = segments[1];
    //        ReturnType = segments[0];
    //    }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }
    //}

    //internal class MethodDescriptorUriElement : DescriptorUriElement
    //{
    //    public string Name { get; set; }
    //    public string ReturnType { get; set; }
    //    public string Arguments { get; set; }

    //    public MethodDescriptorUriElement(string key, string value)
    //        : base(key, value)
    //    {
    //        string[] segments = value.Split(' ', '(', ',', ')');
    //        ReturnType = segments[0];
    //        Name = segments[1];
    //    }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }
    //}


    //internal static class DescriptorUriParser
    //{
    //    public static bool TryParse(string uri, out IDictionary<string, DescriptorUriElement> map)
    //    {
            

    //        string[] groups = uri.Split('&', '?');

    //        map = new Dictionary<string, DescriptorUriElement>(StringComparer.InvariantCultureIgnoreCase);
    //        if (groups.Length < 1)
    //            return false;

    //        map.Add("AssemblyLocation", new DescriptorUriElement("AssemblyLocation", groups[0]));
    //        if (groups.Length < 2)
    //            return true;
            
    //        map.Add("AssemblyName", new DescriptorUriElement("AssemblyName", groups[1].Substring(9)));
    //        for (int i = 2; i < groups.Length; i++)
    //        {
    //            //TODO: validation of groups.
    //            int split = groups[i].IndexOf('=');
    //            string key = groups[i].Substring(0, split);
    //            string value = groups[i].Substring(split + 1);
    //            switch (key.ToLower())
    //            {
    //                case "property":
    //                    map.Add(key, new PropertyDescriptorUriElement(key, value));
    //                    break;
    //                //public string Type { get { return map["type"]; } }
    //                //public string Field { get { return map["field"]; } }
    //                //public string Property { get { return map["property"]; } }
    //                //public string Event { get { return map["event"]; } }
    //                case "method":
    //                    map.Add(key, new MethodDescriptorUriElement(key, value));
    //                    break;
    //                //method=System.String get_StringProperty()
    //                //public string Method { get { return map["method"]; } }
    //                //public string Constructor { get { return map["ctor"]; } }
    //                //public string Module { get { return map["module"]; } }
    //                default:
    //                    map.Add(key, new DescriptorUriElement(key, value));
    //                    break;
    //            }

    //        }

    //        return true;
    //    }


    //    public static DescriptorType GetDescriptorType(IDictionary<string, DescriptorUriElement> map)
    //    {
    //        if (map.ContainsKey("type"))
    //        {
    //            if (map.ContainsKey("field")) return DescriptorType.Field;
    //            if (map.ContainsKey("property")) return DescriptorType.Property;
    //            if (map.ContainsKey("event")) return DescriptorType.Event;
    //            if (map.ContainsKey("method")) return DescriptorType.Method;
    //            if (map.ContainsKey("ctor")) return DescriptorType.Constructor;
    //            return DescriptorType.Type;
    //        }
    //        if (map.ContainsKey("module"))
    //            return DescriptorType.Module;
            
    //        return DescriptorType.Assembly;
    //    }
    //}










}