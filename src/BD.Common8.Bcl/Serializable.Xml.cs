using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace System;

partial class Serializable
{
    static readonly ConcurrentDictionary<Type, XmlSerializer> xmlSerializers = new(); // 缓存实例，避免多次创建

    /// <summary>
    /// 根据类型获取 <see cref="XmlSerializer"/>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static XmlSerializer GetXmlSerializer([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        if (!xmlSerializers.TryGetValue(type, out var value))
        {
            value = new XmlSerializer(type);
            xmlSerializers.TryAdd(type, value);
        }
        return value;
    }
}
