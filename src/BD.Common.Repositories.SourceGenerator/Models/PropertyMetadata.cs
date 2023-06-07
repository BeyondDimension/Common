namespace BD.Common.Repositories.SourceGenerator.Models;

/// <summary>
/// 属性元数据
/// </summary>
/// <param name="PropertyType"></param>
/// <param name="Name"></param>
public record struct PropertyMetadata(string PropertyType, string Name)
{
    /// <summary>
    /// 从 <see cref="IFieldSymbol"/> 中读取属性元数据
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PropertyMetadata Parse(IFieldSymbol field)
    {
        // 从 IFieldSymbol 中解析需要的内容，通过此模型类去生成源码，解析仅执行一次
        var propertyType = field.Type.ToDisplayString();
        var indexOf = propertyType.LastIndexOf('.');
        if (indexOf >= 0)
        {
            propertyType = propertyType[(indexOf + 1)..];
        }
        return new(propertyType, field.Name);
    }

    /// <summary>
    /// 从多个 <see cref="IFieldSymbol"/> 中读取属性元数据
    /// </summary>
    /// <param name="fields"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ImmutableArray<PropertyMetadata> Parse(ImmutableArray<IFieldSymbol> fields)
        => fields.Select(Parse).ToImmutableArray();

    /// <summary>
    /// 将属性元数据写入到源码文件流中
    /// </summary>
    /// <param name="stream"></param>
    public readonly void Write(Stream stream)
    {
        var property =
"""
    public {0} {1} { get; set; }

"""u8;
        stream.WriteFormat(property, PropertyType, Name);
    }

    /// <summary>
    /// 将属性元数据写入到源码文件流中，并根据下标与长度判断是否为最后一个不添加额外的空行
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="i"></param>
    /// <param name="length"></param>
    public readonly void Write(Stream stream, int i, int length)
    {
        Write(stream);
        if (i < length - 1)
        {
            stream.Write(
"""


"""u8);
        }
    }
}
